﻿using Assets.Code.Agent;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using Assets.Code.Utilities;
using BDIMaAssets.Code.ReasoningCycle;
using BDIManager.Beliefs;
using BDIManager.Desires;
using BDIManager.Intentions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/*
 * Implements the reasoning cycle. There are 10 steps in the cycle: 
 * 
 * 1) Perceiving the environment
 * 2) Update de belief base
 * 3) Receiving communication from other agents
 * 4) Selecting 'socially acceptable' messages
 * 5) Selecting an event
 * 6) Retrieving all relevant plans
 * 7) Determining the applicable plans
 * 8) Selecting one applicable plan
 * 9) Selecting an intention for further execution
 * 10) Executing one step of an intention
 * 11) Final stage before restarting the cycle. This isn't really a step but is also necesary. 
 * */

namespace Assets.Code.ReasoningCycle
{
    class Reasoner
    {
        public enum State { StartRC, SelEv, RelPl, ApplPl, SelAppl, FindOp, AddIM, ProcAct, SelInt, ExecInt, ClrInt }


        private Assets.Code.Agent.Agent ag = null; 
        private AgentArchitecture agArch = null;
        private Settings settings = null;
        private Circumstance circumstance = null;

        private State stepSense = State.StartRC;
        private State stepDeliberate = State.SelEv;
        private State stepAct = State.ProcAct;

        private List<Desire> desireListeners;

        private bool sleepingEvt = false;


        private int nrcslbr = Settings.DEFAULT_NUMBER_REASONING_CYCLES; //number of reasoning cycles since last belief revision

        //I don't understand this. Both conf and confP point to this object, this is just to make it look more like the SOS. What is the SOS??
        private Reasoner conf;
        private Reasoner confP;

        //Make a runnable interface and implement it as an innner class
        //private ConcurrentQueue<Runnable> taskForBeginOfCycle = new ConcurrentQueue(); - I don't know how to use this
        private ConcurrentQueue<Runnable> taskForBeginOfCycle = new ConcurrentQueue<Runnable>();

        private Dictionary<Desire, CircumstanceListener> listenersMap; //Map the circumstance listeners created for the goal listeners, used in remove goal listeners

        // the semantic rules are referred to in comments in the functions below
        //private const string kqmlReceivedFunctor = Config.get().getKqmlFunctor();

        private static readonly Atom aNOCODE = new Atom("no_code");


        public Reasoner(Agent.Agent agent, Circumstance c, AgentArchitecture ar, Settings s)
        {
            ag = agent;
            agArch = ar;

            if (s == null)
            {
                s = new Settings();
            } else
            {
                settings = s;
            }

            if (c == null)
            {
                circumstance = new Circumstance();
            } else
            {
                circumstance = c;
            }
            circumstance.SetReasoner(this);
            conf = confP = this;
            if (agent != null) agent.SetReasoner(this);
            if (ar != null) ar.SetReasoner(this);
        }

        //Desire listeners support methods

        //Adds an object that will be notified about events on desires (creation, suspension...)
        public void AddDesireListener(Desire desire)
        {
            if(desireListeners == null)
            {
                desireListeners = new List<Desire>();
                listenersMap = new Dictionary<Desire, CircumstanceListener>();
            } else
            {
                //To not instantiate two DesireListenerForMetaEvents
                foreach(Desire d in desireListeners)
                {
                    if(d is DefaultDesire)
                    {
                        return; 
                    }
                }

                CircumstanceListener cl = new CLImplementation(desire);
                circumstance.AddEventListener(cl);
                listenersMap.Add(desire, cl);
                desireListeners.Add(desire);
            }
        }

        public bool HasGoalListener()
        {
            return desireListeners != null && !desireListeners.Any();
        }

        public List<Desire> GetDesiresListeners()
        {
            return desireListeners;
        }

        public bool RemoveDesireListener(Desire desire)
        {
            CircumstanceListener cl = listenersMap[desire];
            if(cl != null)
            {
                circumstance.RemoveEventListener(cl);
            }

            return desireListeners.Remove(desire);
        }

        /*
         It's the main loop
         Infinite loop on one reasoning cuclle
         plus the other parts of the agent architecture besides
         the actual transition system of the AS interpreter
         */

        public void ReasoningCycle()
        {
            Sense();
            Deliberate();
            Act();
        }

        /*
         Senses the environment
         */
        public void Sense()
        {
            try
            {
                //Start new reasoning cycle 
                GetUserAgArch().ReasoningCycleStarting();
                circumstance.ResetSense();

                if(nrcslbr >= settings.Nrcbp())
                {
                    nrcslbr = 0;
                    //I don't know how to do this
                    /* 
                        synchronized (C.syncApPlanSense) {
                            ag.Buff(GetUserAgArch().Perceive());
                        }
                    */
                    GetUserAgArch().CheckMail();
                }
                nrcslbr++; //Counting number of cycles since last belief revision

                //Produce sleep events
                if(CanSleep())
                {
                    if (!sleepingEvt)
                    {
                        if(ag.GetPlanLibrary().GetCandidatePlans(PlanLibrary.TE_JAG_SLEEPING)!=null)
                        {
                            circumstance.AddExternalEv(PlanLibrary.TE_JAG_SLEEPING);
                        }
                    } else
                    {
                        //I don't know why this is commented
                        //GetUserAgArch().Sleep();
                    }
                } else if (sleepingEvt) //Code to turn idleEvent false again
                {
                    if(circumstance.HasMsg())
                    {
                        sleepingEvt = false;
                    } else if (circumstance.HasEvt())
                    {
                        //Check if there is an event in C.E not produced by idle intention
                        foreach (Event e in circumstance.GetEvents()) 
                        {
                            Intention i = e.GetIntention();
                            if( !e.GetTrigger().Equals(PlanLibrary.TE_JAG_SLEEPING) || i != null && i.HasTrigger(PlanLibrary.TE_JAG_SLEEPING, new Unifier()))
                            {
                                sleepingEvt = false;
                                break;
                            }
                        }
                    }
                    if (!sleepingEvt && ag.GetPlanLibrary().GetCandidatePlans(PlanLibrary.TE_JAG_AWAKING) != null)
                    {
                        circumstance.AddExternalEv(PlanLibrary.TE_JAG_AWAKING);
                    }
                }

                stepSense = State.StartRC;

                do
                {
                    ApplySemanticRuleSense();
                } while (stepSense != State.SelEv && GetUserAgArch().IsRunning());
            } catch (Exception e)
            {
                //print(e.StackTrace());
                conf.GetCircumstance().Create();
            }
        } 

        public void Deliberate()
        {
            try
            {
                circumstance.ResetDeliberate();
                // run tasks allocated to be performed in the begin of the cycle
                Runnable r;
                while(taskForBeginOfCycle.TryDequeue(out r))
                {
                    r.Run();
                }
                
                // Runnable r = taskForBeginOfCycle.poll();
                // while(r != null) 
                // {
                    // r.run(); //It is processed only things related to operations on goals/intentions resumed/suspended/finished It can be placed in the deliberate stage, but the problem is the sleep when the synchronous execution is adopted
                    // r = taskForBeginOfCycle.poll();
                // }
                stepDeliberate = State.SelEv;
                do
                {
                    ApplySemanticRuleDeliberate();
                } while (stepDeliberate != State.ProcAct && GetUserAgArch().IsRunning());
            }
            catch (Exception e)
            {
                //print(e.StackTrace());
                conf.GetCircumstance().Create();
            }
        }

        public void Act()
        {
            try
            {
                circumstance.ResetAct();
                stepAct = State.ProcAct;
                do
                {
                    ApplySemanticRuleAct();
                } while (stepAct != State.StartRC && GetUserAgArch().IsRunning());

                ExecuteAction action = circumstance.GetAction();
                if (action != null)
                {
                    circumstance.AddPendingAction(action);
                    // We need to send a wrapper for FA to the user so that add method then calls C.addFA (which control atomic things)
                    GetUserAgArch().Act(action);
                }
            } catch (Exception e)
            {
                // print(e.StackTrace());
                conf.GetCircumstance().Create();
            }
        }

        public bool CanSleep()
        {
            return (circumstance.IsAtomicIntentionSuspended() && !circumstance.HasFeedbackAction() && !conf.GetCircumstance().HasMsg())  // atomic case
                  || (!conf.GetCircumstance().HasEvent() &&    // other cases (deliberate)
                      !conf.GetCircumstance().HasRunningIntention() && !conf.GetCircumstance().HasFeedbackAction() && // (action)
                      !conf.GetCircumstance().HasMsg() &&  // (sense)
                      taskForBeginOfCycle.IsEmpty &&
                      GetUserAgArch().CanSleep());
        }

        private void ApplySemanticRuleSense()
        {
            switch (stepSense)
            {
                case State.StartRC:
                    ApplyProcMsg();
                    break;
                default:
                    break;
            }
        }

        private void ApplySemanticRuleDeliberate()
        {
            switch (stepDeliberate)
            {
                case State.SelEv:
                    ApplySelEv();
                    break;
                case State.RelPl:
                    ApplyRelPl();
                    break;
                case State.ApplPl:
                    ApplyApplPl();
                    break;
                case State.SelAppl:
                    ApplySelAppl();
                    break;
                case State.FindOp:
                    ApplyFindOp();
                    break;
                case State.AddIM:
                    ApplyAddIP();
                    break;
                default:
                    break;
            }
        }

        public void ApplySemanticRuleAct()
        {
            switch (stepAct)
            {
                case State.ProcAct:
                    ApplyProcAct();
                    break;
                case State.SelInt:
                    ApplySelInt();
                    break;
                case State.ExecInt:
                    ApplyExecInt();
                    break;
                case State.ClrInt:
                    confP.stepAct = State.StartRC;
                    ApplyClrInt(conf.GetCircumstance().GetSI());
                    break;
                default:
                    break;
            }
        }

        public void ApplyClrInt(Intention i)
        {
            while (true) 
            {
                if (i == null)
                {
                    return;
                } 

                if (i.IsFinished())
                {
                    confP.GetCircumstance().DropRunningIntention(i);
                    return;
                }

                IntendedPlan ip = i.Peek();
                if(!ip.IsFinished())
                {
                    return;
                }

                IntendedPlan topIP = i.Pop();
                Trigger topTrigger = topIP.GetTrigger();
                Literal topLiteral = topTrigger.GetLiteral();
                //if (logger.isLoggable(Level.FINE))
                //{
                //logger.fine("Returning from IM " + topIM.getPlan().getLabel() + ", te=" + topTrigger + " unif=" + topIM.unif);
                //}
                if(!topTrigger.IsMetaEvent() && topTrigger.IsGoal() && HasGoalListener())
                {
                    foreach (Desire desire in desireListeners)
                    {
                        desire.DesireFinished(topTrigger, FinishStates.achieved);
                    }
                }

                if(ip.GetTrigger().IsGoal() && !ip.GetTrigger().IsAddition() && !i.IsFinished())
                {
                    ip = i.Peek();
                    if (ip.IsFinished() || !(ip.GetUnif().Unifies(ip.GetCurrentStep().GetBodyTerm(), topLiteral) && ip.GetCurrentStep().GetBodyType() == PlanBody.BodyType.achieve) ||
                        ip.GetCurrentStep().GetBodyTerm().GetType() == typeof(Var))
                    {
                        ip.Pop();
                    }
                    while (!i.IsFinished() && !(ip.GetUnif().Unifies(ip.GetTrigger().GetLiteral(), topLiteral) && ip.GetTrigger().IsGoal())
                        && !(ip.GetUnif().Unifies(ip.GetCurrentStep().GetBodyTerm(), topLiteral) && ip.GetCurrentStep().GetBodyType() == PlanBody.BodyType.achieve)){
                        ip = i.Pop();
                    }
                }

                if (!i.IsFinished())
                {
                    ip = i.Peek();
                    if (!ip.IsFinished())
                    {
                        JoinRenamedVarsIntoIntentionUnifier(ip, topIP.GetUnif());
                        ip.RemoveCurrentStep();
                    }
                }
            }
        }

        private void JoinRenamedVarsIntoIntentionUnifier(IntendedPlan ip, Unifier values)
        {
            if (ip.GetRenamedVars() != null)
            {
                foreach (Var var in ip.GetRenamedVars().Function().KeySet())
                {
                    UnnamedVar vt = (UnnamedVar)ip.GetRenamedVars().Function().Get(var);
                    ip.GetUnif().Unifies(var, vt);
                    Term vl = values.GetFunction().Get(vt);
                    if(vl != null)
                    {
                        vl = vl.Capply(values);
                        if(vl.IsLiteral())
                        {
                            ((Literal)vl).makeVarsAnnon();
                        }
                        ip.GetUnif().Bind(vt, vl);
                    }
                }
            }
        }

        private void ApplyExecInt()
        {
            confP.stepAct = State.ClrInt;

            Intention curInt = conf.GetCircumstance().GetSI();
            if (curInt == null)
            {
                return;
            } 
            if(curInt.IsFinished())
            {
                return;
            }

            IntendedPlan ip = curInt.Peek();

            if (ip.IsFinished())
            {
                UpdateIntention(curInt);
                return;
            }

            Unifier u = ip.GetUnif();
            PlanBody h = ip.GetCurrentStep();
            Term bTerm = h.GetBodyTerm();

            if (bTerm.GetType() == typeof(Var))
            {
                bTerm = bTerm.Cappply(u);
                if (bTerm.IsVar())
                {
                    string msg = h.GetSrcInfo() + ": " + "Variable '" + bTerm + "' must be ground.";
                    if (!GenerateDesireDeletion(curInt, JasonException.createBasicErrorAnnots("body_var_without_value", msg)))
                    {
                        //logger.log(Level.SEVERE, msg);
                    }
                    return;
                }
                if (bTerm.IsPlanBody())
                {
                    if (h.GetBodyType() != BodyType.action)
                    {
                        String msg = h.GetSrcInfo() + ": " + "The operator '" + h.GetBodyType() + "' is lost with the variable '" + bTerm + "' unified with a plan body. ";
                        if (!GenerateDesireDeletion(curInt, JasonException.createBasicErrorAnnots("body_var_with_op", msg)))
                        {
                            //logger.log(Level.SEVERE, msg);
                        }
                        return;
                    }
                }
            }

            if (bTerm.IsPlanBody())
            {
                h = (PlanBody)bTerm;
                if (h.GetPlanSize() > 1)
                {
                    h = (PlanBody)bTerm.Clone();
                    h.Add(ip.GetCurrentStep().GetBodyNext());
                    ip.InsertAsNextStep(h.GetBodyNext());
                }
                bTerm = h.GetBodyTerm();
            }

            Literal body = null;
            if (bTerm.GetType() == typeof(Literal))
            {
                body = (Literal)bTerm;
            }

            switch (h.GetBodyType())
            {
                case PlanBody.BodyType.none:
                    break;
                case PlanBody.BodyType.action:
                    body = (Literal)body.Capply(u);
                    confP.GetCircumstance().setA(new ExecuteAction(body, curInt));
                    break;
                case PlanBody.BodyType.internalAction:
                    bool ok = false;
                    List<Term> errorAnnots = null;
                    try
                    {
                        InternalActions ia = ((InternalAction)bTerm).GetIA(ag);
                        Term[] terms = ia.PrepareArguments(body, u);
                        Object result = ia.Execute(this, u, terms);
                        if (result != null)
                        {
                            ok = result.GetType() == typeof(bool) && (bool)result;
                            if (!ok && result.GetType() == typeof(IEnumerator<Unifier>))
                            {
                                IEnumerator<Unifier> iu = (IEnumerator<Unifier>)result;
                                if (iu.MoveNext())
                                {
                                    ip.SetUnif(iu.Current);
                                    ok = true;
                                }
                            }
                            if (!ok)
                            {
                                //errorAnnots = JasonException.createBasicErrorAnnots("ia_failed", "");
                            }
                        }
                        if (ok && ia.SuspendedIntention())
                        {
                            UpdateIntention(curInt);
                        }
                    }
                    catch (NoValueException e)
                    {
                        string msg = e.getMessage() + " Ungrounded variables = [";
                        string v = "";
                        foreach (Var var in body.GetSingletonVars())
                        {
                            if (u.Get(var) == null)
                            {
                                msg += v + var;
                                v = ",";
                            }
                        }
                        msg += "].";
                        e = new NoValueException(msg);
                        errorAnnots = e.GetErrorTerms();
                        if (!GenerateDesireDeletion(curInt, errorAnnots))
                        {
                            //logger.log(Level.SEVERE, body.getErrorMsg() + ": " + e.getMessage());
                        }

                        ok = true;

                    }
                    catch (JasonException e)
                    {
                        errorAnnots = e.getErrorTerms();
                        if (!GenerateDesireDeletion(curInt, errorAnnots))
                        {
                            //logger.log(Level.SEVERE, body.getErrorMsg() + ": " + e.getMessage());
                        }
                        ok = true; 
                    }
                    catch (Exception e)
                    {
                        if (body == null)
                        {
                            //logger.log(Level.SEVERE, "Selected an intention with null body in '" + h + "' and IM " + im, e);
                        }
                        else
                        {
                            // logger.log(Level.SEVERE, body.getErrorMsg() + ": " + e.getMessage(), e);
                        }
                    }
                    if (!ok)
                    {
                        GenerateDesireDeletion(curInt, errorAnnots);
                    }
                    break;

                case PlanBody.BodyType.constraint:
                    IEnumerator<Unifier> iu = ((LogicalFormula)bTerm).LogicalConsecuence(ag, u);
                    if (iu.MoveNext())
                    {
                        ip.SetUnif(iu.Current);
                        UpdateIntention(curInt);
                    } else
                    {
                        string msg = "Constraint " + h + " was not satisfied (" + h.GetSrcInfo() + ") un=" + u;
                        GenerateDesireDeletion(curInt, JasonException.createBasicErrorAnnots(new Atom("constraint_failed"), msg));
                        //logger.fine(msg);
                    }
                    break;

                case PlanBody.BodyType.achieve:
                    body = PrepareBodyForEvent(body, u, curInt.Peek());
                    Event evt = conf.GetCircumstance().AddAchieveDesire(body, curInt);
                    confP.stepAct = State.StartRC;
                    CheckHardDeadline(evt);
                    break;

                case PlanBody.BodyType.achieveNF:
                    body = PrepareBodyForEvent(body, u, null);
                    evt = conf.GetCircumstance().AddAchieveDesire(body, Intention.emptyInt);
                    CheckHardDeadline(evt);
                    UpdateIntention(curInt);
                    break;

                case PlanBody.BodyType.test:
                    LogicalFormula f = (LogicalFormula)bTerm;
                    if(conf.GetAgent().Believes(f, u))
                    {
                        UpdateIntention(curInt);
                    } else
                    {
                        bool fail = true;
                        if (f.IsLiteral() && f.GetType() != typeof(BinaryStructure))
                        {
                            body = PrepareBodyForEvent(body, u, curInt.Peek());
                            if (body.IsLiteral())
                            {
                                Trigger t = new Trigger(TEOperator.add, TEType.test, body);
                                evt = new Event(t, curInt);
                                if(ag.GetPlanLibrary().HasCandidatePlan(t))
                                {
                                    //if (logger.isLoggable(Level.FINE)) 
                                    //{
                                     //   logger.fine("Test Goal '" + bTerm + "' failed as simple query. Generating internal event for it: " + te);
                                    //}
                                    conf.GetCircumstance().addEvent(evt);
                                    confP.stepAct = State.StartRC;
                                    fail = false;
                                }
                            }
                        }
                        if(fail)
                        {
                           // if (logger.isLoggable(Level.FINE))
                           // {
                            //    logger.fine("Test '" + bTerm + "' failed (" + h.getSrcInfo() + ").");
                          //  }
                            GenerateDesireDeletion(curInt, JasonException.createBasicErrorAnnots("test_goal_failed", "Failed to test '" + bTerm + "'"));
                        }
                    }
                    break;

                case PlanBody.BodyType.delAddBel:
                    Literal b2 = PrepareBodyForEvent(body, u, curInt.Peek());
                    b2.MakeTermsAnnon();
                    try
                    {
                        List<Literal>[] result = ag.brf(null, b2, curInt);
                        if (result != null)
                        {
                            UpdateEvents(result, Intention.emptyInt);
                        }
                    } catch (RevisionFailedException re)
                    {
                        GenerateDesireDeletion(curInt, JasonException.createBasicErrorAnnots("belief_revision_failed", "BRF failed for '" + body + "'"));
                        break;
                    }

                    //The original one doesn't have the break, but if i don't put it here there are compiler errors for some misterious reasons
                    break;

                case PlanBody.BodyType.addBel:
                case PlanBody.BodyType.addBelBegin:
                case PlanBody.BodyType.addBelEnd:
                case PlanBody.BodyType.addBelNewFocus:
                    //But here i don't have breaks and all works because there's no logic anywhere
                    Intention newFocus = Intention.emptyInt;
                    Boolean isSameFocus = GetSettings().SameFocus() && h.GetBodyType() != PlanBody.BodyType.addBelNewFocus;
                    if (isSameFocus)
                    {
                        newFocus = curInt;
                        body = PrepareBodyForEvent(body, u, newFocus.Peek());
                    } else
                    {
                        body = PrepareBodyForEvent(body, u, null);
                    }

                    try
                    {
                        List<Literal>[] result;
                        if (h.GetBodyType() == PlanBody.BodyType.addBelEnd)
                        {
                            result = GetAgent().brf(body, null, curInt, true);
                        }
                        else
                        {
                            result = GetAgent().brf(body, null, curInt);
                        }
                        if (result != null)
                        {
                            UpdateEvents(result, newFocus);
                            if(!isSameFocus)
                            {
                                UpdateIntention(curInt);
                            }
                        } else
                        {
                            UpdateIntention(curInt);
                        }
                    } catch (RevisionFailedException re)
                    {
                        GenerateDesireDeletion(curInt, null);
                    }

                    break;

                case PlanBody.BodyType.delBelNewFocus:
                case PlanBody.BodyType.delBel:
                    newFocus = Intention.emptyInt; //Here it isnt declare because why not
                    isSameFocus = GetSettings().SameFocus() && h.GetBodyType() != PlanBody.BodyType.delBelNewFocus;
                    if (isSameFocus)
                    {
                        newFocus = curInt;
                        body = PrepareBodyForEvent(body, u, newFocus.Peek());
                    } else
                    {
                        body = PrepareBodyForEvent(body, u, null);
                    } 
                    try
                    {
                        List<Literal>[] result = GetAgent().brf(null, body, curInt);
                        if (result != null)
                        {
                            UpdateEvents(result, newFocus);
                            if(!isSameFocus)
                            {
                                UpdateIntention(curInt);
                            }
                        } else {
                            UpdateIntention(curInt);
                        }
                    } catch(RevisionFailedEception re)
                    {
                        GenerateDesireDeletion(curInt, null);
                    }
                    break;
            }
        }

        public void UpdateEvents(List<Literal>[] result, Intention focus)
        {
            if (result == null)
            {
                return;
            }

            foreach (Literal lAdd in result[0])
            {
                Trigger t = new Trigger(TEOperator.add, TEType.belief, lAdd);
                UpdateEvents(new Event(t, focus));
                focus = Intention.emptyInt;
            }

            foreach (Literal lRem in result[1])
            {
                Trigger t = new Trigger(TEOperator.del, TEType.belief, lRem);
                UpdateEvents(new Event(t, focus));
                focus = Intention.emptyInt;
            }
        }

        public void UpdateEvents(Event ev)
        {
            if (ev.IsInternal() || GetCircumstance().HasListener() || ag.GetPlanLibrary().HasCandidatePlan(ev.GetTrigger()))
            {
                GetCircumstance().AddEvent(ev);
                //if (logger.isLoggable(Level.FINE)) logger.fine("Added event " + e+ ", events = "+C.getEvents());
            }
        }

        private Literal PrepareBodyForEvent(Literal body, Unifier u, IntendedPlan ipRenamedVars)
        {
            body = (Literal)body.Capply(u);
            Unifier renamedVars = new Unifier();
            body.MakeVarsAnnon(renamedVars);
            if (ipRenamedVars != null)
            {
                ipRenamedVars.SetRenamedVars(renamedVars);
                if (GetSettings().IsTROon())
                {
                    Dictionary<VarTerm, Term> adds = null;
                    foreach (VarTerm v in renamedVars)
                    {
                        Term t = u.Function().Get(v);
                        if (t != null && t.IsVar())
                        {
                            if (adds == null)
                                adds = new Dictionary<VarTerm, Term>();
                            try
                            {
                                adds.Add((VarTerm)t, renamedVars.Function().Get(v));
                            } catch (Exception e)
                            {
                                //logger.log(Level.SEVERE, "*** Error adding var into renamed vars. var=" + v + ", value=" + t + ".", e);
                            }
                        }
                    }
                    if (adds != null)
                    {
                        renamedVars.Function().PutAll(adds);
                    }
                }
            }

            body = body.ForceFullLiteralImpl();
            if(!body.HasSource())
            {
                body.AddAnnot(BeliefBase.TSelf);
            }
            return body;
        }

        private void CheckHardDeadline(Event evt)
        {
            Literal body = evt.GetTrigger().GetLiteral();
            Literal hdl = body.GetAnnot(/*ASSyntax.*/GetHardDeadLineStr);
            if (hdl == null)
            {
                return;
            }
            if (hdl.GetArity() < 1)
            {
                return;
            }

            Intention i = evt.GetIntention();
            int iSize;
            if (i == null)
            {
                iSize = 0;
            }
            else
            {
                iSize = i.Size();
            }
            int deadline = 0;
            try
            {
                deadline = (int)((NumberTerm)hdl.GetTerm(0)).Solve(); //I have to look this 
            }
            catch (NoValueException e)
            {

            }

            Agent.GetScheduler().Schedule(new RunnableImpl(), deadline, TimeUnit.MILLISECONDS);

        }

        public bool GenerateDesireDeletion(Intention i, List<Term> errorAnnots)
        {
            bool failEventIsRelevant = false;
            IntendedPlan ip = i.Peek();
            Event failEvent = FindEventForFailure(i, ip.GetTrigger());
            if (failEvent != null)
            {
                failEventIsRelevant = true;
            } else
            {
                failEvent = new Event(ip.GetTrigger().Clone(), i);
            }

            Term bodyPart = ip.GetCurrentStep().GetBodyTerm().Capply(ip.GetUnif());
            SetDefaultFailureAnnots(failEvent, bodyPart, errorAnnots);
            if (ip.IsGoalAdd())
            {
                if (HasGoalListener())
                {
                    foreach (Desire desire in desireListeners)
                    {
                        desire.DesireFailed(ip.GetTrigger());
                        if (!failEventIsRelevant)
                        {
                            desire.DesireFinished(ip.GetTrigger(), FinishStates.unachieved);
                        }
                    }
                }

                if (failEventIsRelevant)
                {
                    confP.GetCircumstance().AddEvent(failEvent);
                    //if (logger.isLoggable(Level.FINE)) logger.fine("Generating goal deletion " + failEvent.getTrigger() + " from goal: " + im.getTrigger());
                }
                else
                {
                    //logger.warning("No failure event was generated for " + failEvent.getTrigger() + "\n" + i);
                    i.Fail(GetCircumstance());
                }
            } else if(GetSettings().Requeue())
            {
                ip = ip.Peek();
                confP.GetCircumstance().AddExternalEv(ip.GetTrigger());
            } else
            {
                //logger.warning("Could not finish intention: " + i + "\tTrigger: " + failEvent.getTrigger());
            }
            return failEventIsRelevant;
        }

        private static void SetDefaultFailureAnnots(Event failEvent, Term body, List<Term> errorAnnots)
        {
            if (errorAnnots == null)
            {
                errorAnnots = JasonException.createBasicErrorAnnots(JasonException.UNKNOW_ERROR, "");
            }

            Literal eLiteral = failEvent.GetTrigger().GetLiteral().ForceFullLiteralImpl();
            eLiteral.AddAnnot(errorAnnots);

            Literal bodyTerm = aNOCODE;
            Term codesrc = aNOCODE;
            Term codeline = aNOCODE;
            if (body != null && body.GetType() == typeof(Literal))
            {
                bodyTerm = (Literal)body;
                if (bodyTerm.GetSrcInfo() != null)
                {
                    if(bodyTerm.GetSrcInfo().GetSrcFile() != null)
                    {
                        codesrc = new StringTermImpl(bodyTerm.GetSrcInfo().GetSrcFile());
                    }
                }
                codeline = new NumberTermImpl(bodyTerm.GetSrcInfo().GetSrcLine());
            }

            if (eLiteral.GetAnnot("code") == null)
            {
                eLiteral.AddAnnot(/*ASSyntax.*/CreateStructure("code", bodyTerm.Copy().MakeVarsAnnon()));
            }

            if (eLiteral.GetAnnot("code_src") == null)
            {
                eLiteral.AddAnnot(/*ASSyntax.*/CreateStructure("code_src", codesrc));
            }

            if(eLiteral.GetAnnot("code_line") == null)
            {
                eLiteral.AddAnnot(/*ASSyntax.*/CreateStructure("code_line", codeline));
            }
        }

        public Event FindEventForFailure(Intention i, Trigger trigger)
        {
            if (i != Intention.emptyInt)
            {
                return i.FindEventForFailure(trigger, GetAgent().GetPlanLibrary(), GetCircumstance().GetFirst());
            } else if (trigger.IsGoal() && trigger.IsAddition())
            {
                Trigger failTrigger = new Trigger(TEOperator.del, trigger.GetType(), trigger.GetLiteral());
                if(GetAgent().GetPlanLibrary().HasCandidatePlan(failTrigger))
                {
                    return new Event(failTrigger.Clone(), i);
                }
            }
            return null;
        }

        private void UpdateIntention(Intention i)
        {
            if (!i.IsFinished())
            {
                i.Peek().RemoveCurrentStep();
                confP.GetCircumstance().AddRunningIntention(i);
            } else
            {
                //logger.fine("trying to update a finished intention!");
            }
        }

        private void ApplySelInt()
        {
            confP.stepAct = State.ExecInt;

            confP.GetCircumstance().SetSI(GetCircumstance().RemoveAtomicIntention());
            if (confP.GetCircumstance().GetSI() != null)
            {
                return;
            }

            if (!conf.GetCircumstance().IsAtomicIntentionSuspended() && conf.GetCircumstance().HasRunningIntention())
            {
                confP.GetCircumstance().SetSI(conf.GetAgent().SelectIntention(conf.GetCircumstance().GetRunningIntentions()));
                //if (logger.isLoggable(Level.FINE)) logger.fine("Selected intention " + confP.C.SI);
                if (confP.GetCircumstance().GetSI() != null)
                { 
                    return;
                }
            }

            confP.stepAct = State.StartRC;
        }

        private void ApplyProcAct()
        {
            confP.stepAct = State.SelInt;
            if (conf.GetCircumstance().HasFeedbackAction())
            {
                ExecuteAction a = null;
                /*synchronized (conf.GetCircumstance().GetFeedbackActions()){
                    a = conf.GetAgent().SelectAction(conf.GetCircumstance().GetFeedbackActions());
                }*/

                if (a != null)
                {
                    Intention curInt = a.GetIntention();
                
                    if (GetCircumstance().RemovePendingIntention(curInt.GetID()) != null) {
                        if (a.GetResult())
                        {
                            UpdateIntention(curInt);
                            ApplyClrInt(curInt);

                            if (HasGoalListener())
                            {
                                foreach (Desire desire in GetDesiresListeners())
                                {
                                    foreach (IntendedPlan ip in curInt.GetIntendedPlan())
                                    {
                                        desire.DesireResumed(ip.GetTrigger());
                                    }
                                }
                            }
                        } else
                        {
                            String reason = a.GetFailureMsg();
                            if (reason == null)
                            {
                                reason = "";
                            }
                            ListTerm annots = JasonException.createBasicErrorAnnots("action_failed", reason);
                            if (a.GetFailureReason() != null)
                            {
                                annots.Append(a.GetFailureReason());
                            }
                            GenerateDesireDeletion(curInt, annots);
                            GetCircumstance().RemoveAtomicIntention();
                        }
                    } else
                    {
                        ApplyProcAct();
                    }
                }
            }
        }

        private void ApplyAddIP()
        {
            //Create a new intended plan
            IntendedPlan ip = new IntendedPlan(conf.GetCircumstance().GetSO(), conf.GetCircumstance().GetSE().GetTrigger());

            //Rule ExtEv
            if (conf.GetCircumstance().GetSE().GetIntention() == Intention.emptyInt)
            {
                Intention intention = new Intention();
                intention.Push(ip);
                confP.GetCircumstance().AddRunningIntention(intention);
            } else
            {
                //Rule IntEv
                if(GetSettings().IsTROon())
                {
                    IntendedPlan top = confP.GetCircumstance().GetSE().GetIntention().Peek();

                    if(top != null && top.GetTrigger().IsAddition() && ip.GetTrigger().IsAddition() && 
                        top.GetTrigger().IsGoal() && ip.GetTrigger().IsGoal() &&
                        top.GetCurrentStep().GetBodyNext() == null &&
                        top.GetTrigger().GetLiteral().GetPredicateIndicator().Equals(ip.GetTrigger().GetLiteral().GetPredicateIndicator()))
                    {
                        confP.GetCircumstance().GetSE().GetIntention().Pop();

                        IntendedPlan ipBase = confP.GetCircumstance().GetSE().GetIntention().Peek();
                        if (ipBase != null && ipBase.GetRenamedVars() != null)
                        {
                            foreach (Var var in ipBase.GetRenamedVars())
                            {
                                Var vl = (Var)ipBase.GetRenamedVars().GetFunction().Get(var);
                                Term t = top.GetUnif().Get(vl);
                                if (t != null)
                                {
                                    if (t.GetType() == typeof(Literal))
                                    {
                                        Literal l = (Literal)t.Capply(top.GetUnif());
                                        l.MakeVarsAnnon(top.GetRenamedVars());
                                        ip.GetUnif().GetFunction().Put(vl, l);
                                    } else
                                    {
                                        ip.GetUnif().GetFunction().Put(vl, t);
                                    }
                                }
                            }
                        }
                    }
                }
                confP.GetCircumstance().GetSE().GetIntention().Push(ip);
                confP.GetCircumstance().AddRunningIntention(confP.GetCircumstance().GetSE().GetIntention());
            }
            confP.stepDeliberate = State.ProcAct;
        }

        private void ApplyFindOp()
        {
            confP.stepDeliberate = State.AddIM;

            List<Plan> candidateRPs = conf.GetAgent().GetPlanLibrary().GetCandidatePlans(conf.GetCircumstance().GetSE().GetTrigger());
            if (candidateRPs != null)
            {
                foreach (Plan p in candidateRPs)
                {
                    Unifier relUn = p.IsRelevant(conf.GetCircumstance().GetSE().GetTrigger());
                    if (relUn != null)
                    {
                        LogicalFormula context = p.GetContext();
                        if (context != null)
                        {
                            confP.GetCircumstance().SetSO(new Option(p, relUn));
                            return;
                        } else
                        {
                            IEnumerator<Unifier> r = context.LogicalConsecuence(ag, relUn);
                            if (r != null && r.MoveNext())
                            {
                                confP.GetCircumstance().SetSO(new Option(p, r.Current));
                                return;
                            }
                        }
                    }
                }
                ApplyRelApplPlRule2("applicable");
            } else
            {
                //Problem: no plan
                ApplyRelApplPlRule2("relevant");
            }
        }

        private void ApplySelAppl()
        {
            //Rule SelAppl
            confP.GetCircumstance().SetSO(conf.GetAgent().HasCustomSelectOption(confP.GetCircumstance().GetAP()));

            if (confP.GetCircumstance().GetSO() != null)
            {
                confP.stepDeliberate = State.AddIM;
                //if (logger.isLoggable(Level.FINE)) logger.fine("Selected option "+confP.C.SO+" for event "+confP.C.SE);
            } else
            {
                //logger.fine("** selectOption returned null!");
                generateGoalDeletionFromEvent(/*JasonException.createBasicErrorAnnots("no_option", "selectOption returned null")*/);
                confP.stepDeliberate = State.ProcAct;
            }
        }

        private void ApplyApplPl()
        {
            confP.GetCircumstance().SetAP(ApplicablePlans(confP.GetCircumstance().GetRp()));

            // Rule Appl1
            if (confP.GetCircumstance().GetAP() != null || GetSettings().Retrieve())
            {
                //Retrieve is mainly for Coo-AgentSpeak
                confP.stepDeliberate = State.SelAppl;
            } else
            {
                ApplyRelApplPlRule2("applicable");
            }
        }

        public List<Option> ApplicablePlans(List<Option> rp)
        {
            //synchronized (GetCircumstance().SyncApplyPlanSense) {
            List<Option> ap = null;
            if(rp != null)
            {
                foreach (Option o in rp) 
                {
                    LogicalFormula context = o.GetPlan().GetContext();
                    if (context == null)
                    {
                        if (ap == null)
                        {
                            ap = new List<Option>();
                        }
                        ap.Add(o);
                    } else
                    {
                        bool allU = o.GetPlan().IsAllUnifs();
                        IEnumerator<Unifier> r = context.LogicalConsecuence(ag, o.getUnifier());
                        if ( r != null)
                        {
                            while(r.MoveNext())
                            {
                                o.SetUnifier(r.Current);
                                if(ap == null)
                                {
                                    ap = new List<Option>();
                                }

                                ap.Add(o);

                                if (!allU)
                                    break;

                                if(r.MoveNext())
                                {
                                    o = new Option(o.GetPlan(), null);
                                }
                            }
                        }
                    }
                }
            }
            return ap;
            //}
        }

        private void ApplyRelPl()
        {
            //Get all relevant plans for the selected event
            confP.GetCircumstance().SetRP(RelevantPlans(conf.GetCircumstance().GetSE().GetTrigger()));

            //Rule Rel1
            if (confP.GetCircumstance().GetRp() != null || GetSettings().Retrieve())
            {
                //Is mainly for coo--AgentSpeak
                confP.stepDeliberate = State.ApplPl;
            } else
            {
                ApplyRelApplPlRule2("relevant");
            }
        }

        //Generates desire deletion event
        private void ApplyRelApplPlRule2(string m)
        {
            confP.stepDeliberate = State.ProcAct; //Default next step
            if (conf.GetCircumstance().GetSE().GetTrigger().IsGoal() && !conf.GetCircumstance().GetSE().GetTrigger().IsMetaEvent())
            {
                //Can't carry on, no relevant/applicable plan
                try
                {
                    if (conf.GetCircumstance().GetSE().GetIntention() != null && conf.GetCircumstance().GetSE().GetIntention().Size() > 3000) //I don't know why is 3000
                    {
                        //logger.warning("we are likely in a problem with event " + conf.C.SE.getTrigger() + " the intention stack has already " + conf.C.SE.getIntention().size() + " intended means!");
                    }
                    string msg = "Found a goal for which there is no " + m + " plan:" + conf.GetCircumstance().GetSE().GetTrigger();
                    if (!generateGoalDeletionFromEvent(/*JasonException.createBasicErrorAnnots("no_" + m, msg)*/))
                    {
                        //logger.warning(msg);
                    }
                } catch (Exception e)
                {
                    return;
                }
            } else if (conf.GetCircumstance().GetSE().IsInternal())
            {
                // e.g. belief addition as internal event, just go ahead
                // but note that the event was relevant, yet it is possible
                // the programmer just wanted to add the belief and it was
                // relevant by chance, so just carry on instead of dropping the
                // intention
                Intention i = conf.GetCircumstance().GetSE().GetIntention();
                JoinRenamedVarsIntoIntentionUnifier(i.Peek(), i.Peek().GetUnif());
                UpdateIntention(i);
            } else if (GetSettings().Requeue())
            {
                confP.GetCircumstance().AddEvent(conf.GetCircumstance().GetSE());
            } else
            {
                confP.stepDeliberate = State.SelEv;
            }
        }

        private bool GenerateDesireDeletionFromEvent(List<Term> failAnnots)
        {
            Event e = conf.GetCircumstance().GetSE();
            if (e == null)
            {
                //logger.warning("** It was impossible to generate a goal deletion event because SE is null! " + conf.C);
                return false;
            }

            Trigger t = e.GetTrigger();
            bool failEventGenerated = false;
            if(t.IsAddition() && t.IsGoal())
            {
                if (HasGoalListener())
                {
                    foreach (Desire d in desireListeners)
                    {
                        d.DesireFailed(t);
                    }
                }

                Event failE = FindEventForFailure(e.GetIntention(), t);
                if (failE != null)
                {
                    SetDefaultFailureAnnots(failE, t.GetLiteral(), failAnnots);
                    confP.GetCircumstance().AddEvent(failE);
                    failEventGenerated = true;
                } else
                {
                    if (e.GetIntention() != null)
                    {
                        e.GetIntention().Fail(GetCircumstance());
                    }
                }
            } else if (e.IsInternal())
            {
                //logger.warning("Could not finish intention:\n" + ev.intention);
            } else if (GetSettings().Requeue())
            {
                confP.GetCircumstance().AddEvent(e);
            } else
            {
                //logger.warning("Discarding external event: " + ev);
            }
            return failEventGenerated;
        }

        public List<Option> RelevantPlans(Trigger trigger)
        {
            Trigger t = trigger.Clone();
            List<Option> rp = null;
            List<Plan> candidateRP = conf.GetAgent().GetPlanLibrary().GetCandidatePlans(t);

            if(candidateRP != null)
            {
                foreach (Plan p in candidateRP)
                {
                    Unifier u = p.IsRelevant(t);
                    if (u != null)
                    {
                        if (rp == null)
                        {
                            rp = new List<Option>();
                        }
                        rp.Add(new Option(p, u));
                    }
                }
            }
            return rp;
        }

        private void ApplySelEv()
        {
            // Rule for atomic, if there is an atomic intention, do not select event
            if (GetCircumstance().HasAtomicIntention())
            {
                confP.stepDeliberate = State.ProcAct; //Need to go to ProcAct to see if an atomic intention received a feedback action
                return;
            }

            // Rule for atomic, events from atomic intention have priority
            confP.GetCircumstance().SetSE(GetCircumstance().RemoveAtomicEvent());
            if(confP.GetCircumstance().GetSE() != null)
            {
                confP.stepDeliberate = State.RelPl;
                return;
            }

            if (conf.GetCircumstance().HasEvent())
            {
                //Rule SelEv1
                confP.GetCircumstance().SetSE(conf.GetAgent().SelectEvent(confP.GetCircumstance().GetEvents()));
                if (confP.GetCircumstance().GetSE() != null)
                {
                    if (GetAgent().HasCustomSelectOption() || GetSettings().Verbose() == 2) //verbose == debug mode
                    {
                        confP.stepDeliberate = State.RelPl;
                    } else
                    {
                        confP.stepDeliberate = State.FindOp;
                    }
                    return;
                }
            }
            //Rule SelEv2 directly to ProcAct if no event to handle
            confP.stepDeliberate = State.ProcAct;
        }

        private void ApplyProcMsg()
        {
            confP.stepSense = State.SelEv;
            if (conf.GetCircumstance().HasMsg())
            {
                Message m = conf.ag.SelectMessage(conf.GetCircumstance().GetMailBox());
                if (m == null)
                {
                    return; //If there's no mail, there's nothing more to do
                }

                //Get the content of the message, it can be any term (literal, list, number, ...;)
                Term content = null;
                if (m.GetPropCont().GetType() == typeof(Term))
                {
                    content = (Term)m.GetPropCont();
                } else
                {
                    try
                    {
                        content = Parser.ParseTerm(m.GetPropCont().ToString()); //This needs to be implemented
                    } catch (Exception e)
                    {
                        content = new ObjectTermImpl(m.GetPropCont()); //This needs to be implemented
                    }
                }

                //Checks if an intention was suspended waiting this message
                Intention intention = null;
                if (m.GetInReplyTo() != null)
                {
                    intention = GetCircumstance().GetPendingIntentions().Get(m.GetInReplyTo());
                }
                //Is it a pending intention?
                if (intention != null)
                {
                    //Unify the message answer with the .send fourth argument.
                    //the send that put the intention in Pending state was
                    //something like
                    //  .send(ag1,askOne,value, X)
                    //if the answer was tell 3, unifies X=3
                    //if the answer was untell 3, unifies X=false
                    Structure send = (Structure)intention.Peek().GetCurrentStep().GetBodyTerm(); //This needs to be implemented
                    if (m.IsUntell() && send.GetTerm(1).ToString().Equals("askOne"))
                    {
                        content = Literal.LFalse;
                    } else if (content.IsLiteral())
                    {
                        content = add_nested_source.AddAnnotToList(content, new Atom(m.GetSender()));
                    } else if (send.GetTerm(1).ToString().Equals("askAll") && content.IsList()) //Adds source in each answer if possible
                    {
                        ListTerm tail = new ListTermImpl();
                        foreach (Term t in ((ListTerm)content))
                        {
                            t = add_nested_source.AddAnnotToList(t, new Atom(m.GetSender()));
                            tail.Append(t);
                        }
                        content = tail;
                    }

                    //Test the case of sync ask with many receivers
                    Unifier un = intention.Peek().GetUnif();
                    Term rec = send.GetTerm(0).Capply(un);
                    if (rec.IsList()) //Send to many receivers
                    {
                        //Put the answers in the unifier
                        Var answers = new Var("AnsList___" + m.GetInReplyTo());
                        ListTerm listOfAnswers = (ListTerm)un.Get(answers);
                        if (listOfAnswers == null)
                        {
                            listOfAnswers = new ListTermImpl();
                            un.Unifies(answers, listOfAnswers);
                        }
                        listOfAnswers.Append(content);
                        int nbReceivers = ((ListTerm)send.GetTerm(0)).Size();
                        if(listOfAnswers.Size() == nbReceivers)
                        {
                            //All agents have answered
                            ResumeSyncAskIntention(m.GetInReplyTo(), send.GetTerm(3), listOfAnswers);
                        }
                    } else
                    {
                        ResumeSyncAskIntention(m.GetInReplyTo(), send.GetTerm(3), content);
                    }
                } else if (conf.ag.SocAcc(m))
                {
                    if (!m.IsReplyToSyncAsk())
                    {
                        //Ignore answer after the timeout
                        String sender = m.GetSender();
                        if (sender.Equals(GetUserAgArch().GetAgName()))
                        {
                            sender = "self";
                        }

                        bool added = false;
                        if (!settings.IsSync() && !ag.GetPlanLibrary().HasUserKqmlReceivedPlans() && content.IsLiteral() && !content.IsList())
                        {
                            //Optimisation to jump kqmlPlans
                            if (m.GetIlForce().Equals("achieve"))
                            {
                                content = add_nested_source.AddAnnotToList(content, new Atom(sender));
                                GetCircumstance().AddEvent(new Event(new Trigger(TEOperator.add, TEType.achieve, (Literal)content), Intention.emptyInt()));
                                added = true;
                            } else if (m.GetIlForce().Equals("tell"))
                            {
                                content = add_nested_source.AddAnnotToList(content, new Atom(sender));
                                GetAgent().AddBel((Literal)content);
                                added = true;
                            }

                        }

                        if (!added)
                        {
                            Literal received = new LiteralImpl(kqmlReceivedFunctor).AddTerms(
                                new Atom(sender),
                                new Atom(m.GetIlForce()),
                                content,
                                new Atom(m.GetMessageId()));

                            UpdateEvents(new Event(new Trigger(TEOPerator.add, TEType.achieve, received), Intention.emptyInt()));
                        }
                    } else
                    {
                        //logger.fine("Ignoring message "+m+" because it is received after the timeout.");
                    }
                }
            }
        }

        private void ResumeSyncAskIntention(String msgId, Term answerVar, Term answerValue)
        {
            Intention i = GetCircumstance().RemovePendingIntention(msgId);
            i.Peek().RemoveCurrentStep();
            if (i.Peek().GetUnif().Unifies(answerVar, answerValue))
            {
                GetCircumstance().ResumeIntention(i);
            } else
            {
                GenerateDesireDeletion(i/*, JasonException.createBasicErrorAnnots("ask_failed", "reply of an ask message ('"+answerValue+"') does not unify with fourth argument of .send ('"+answerVar+"')"))*/);
            }
        }

        private void GenerateDesireDeletion(Intention i)
        {
            throw new NotImplementedException();
        }

        public Agent.Agent GetAgent()
        {
            return ag;
        }

        public Circumstance GetCircumstance()
        {
            return circumstance;
        }

        /*public State getStep() {
            return step;
        }*/

        private void RunAtBeginOfNextCycle(Runnable r)
        {
            taskForBeginOfCycle.Enqueue(r);
        }

        public bool CanSleepSense()
        {
            return !GetCircumstance().HasMsg() && GetUserAgArch().CanSleep();
        }

        public bool CanSleepDeliberate()
        {
            return !GetCircumstance().HasEvent() && taskForBeginOfCycle.IsEmpty && GetCircumstance().GetSelectedEvent() == null && GetUserAgArch().CanSleep();
        }

        public bool CanSleepAct()
        {
            return !GetCircumstance().HasRunningIntention() && !GetCircumstance().HasFeedbackAction() && GetCircumstance().GetSelectedIntention() == null && GetUserAgArch().CanSleep();
        }

        public Settings GetSettings()
        {
            return settings;
        }

        public void SetAgArch(AgentArchitecture arch)
        {
            agArch = arch;
        }

        public AgentArchitecture GetUserAgArch()
        {
            return agArch;
        }

        public override string ToString()   
        {
            return "Reasoning cycle of agent " + GetUserAgArch().GetAgName();
        }

        class FailWithDeadline : fail_goal
        {
            Intention intToDrop;
            Trigger t;

            public FailWithDeadline(Intention inten, Trigger te)
            {
                intToDrop = inten;
                t = te;
            }

            public int DropDesire(Intention i, Trigger te, Reasoner r, Unifier u)
            {
                if (i != null)
                {
                    if (intToDrop != null)
                    {
                        if (t != i.GetBottom().GetTrigger())
                        {
                            return 0;
                        }
                    } else if (!intToDrop.Equals(i))
                    {
                        return 0;
                    }

                    if (i.DropDesire(te, u))
                    {
                        if (r.HasGoalListener())
                        {
                            foreach (Desire d in r.GetDesiresListeners())
                            {
                                d.DesireFailed(te);
                            }
                        }

                        Event failEvent = r.FindEventForFailure(i, te);
                        if (failEvent != null)
                        {
                            failEvent.GetTrigger().GetLiteral().AddAnnot(JasonException.createBasicErrorAnnots("deadline_reached", ""));
                            r.GetCircumstance().AddEvent(failEvent);
                            //ts.getLogger().fine("'hard_deadline(" + g + ")' is generating a goal deletion event: " + failEvent.getTrigger());
                            return 2;
                        } else
                        {
                            //ts.getLogger().fine("'hard_deadline(" + g + ")' is removing the intention without event:\n" + i);
                            return 3;
                        }
                    }
                    return 0;
                }
            }
        }

        /*
         This innner class is here to imitate the anonymous interface implementation that exist on Java but not here
         */

        private class CLImplementation : CircumstanceListener
        {
            private Desire d;

            public CLImplementation(Desire desire)
            {
                d = desire;
            }

            public void EventAdded(Event e)
            {
                if (e.GetTrigger().IsAddition() && e.GetTrigger().IsGoal())
                    d.DesireStarted(e);
            }

            public void IntentionAdded(Intention i)
            {
                
            }

            public void IntentionDropped(Intention i)
            {
                foreach (IntendedPlan ip in i.GetIntendedPlan())
                {
                    if (ip.GetTrigger().IsAddition() && ip.GetTrigger().IsGoal())
                        d.DesireFinished(ip.GetTrigger(), FinishStates.dropped);
                }
            }

            public void IntentionResumed(Intention i)
            {
                foreach (IntendedPlan ip in i.GetIntendedPlan())
                {
                    if (ip.GetTrigger().IsAddition() && ip.GetTrigger().IsGoal())
                        d.DesireResumed(ip.GetTrigger());
                }
            }

            public void IntentionSuspended(Intention i, string reason)
            {
                foreach (IntendedPlan ip in i.GetIntendedPlan())
                {
                    if (ip.GetTrigger().IsAddition() && ip.GetTrigger().IsGoal())
                        d.DesireSuspended(ip.GetTrigger(), reason);
                }
            }
        }

        private class RunnableImpl : Runnable
        {
            Intention i;
            Desire d;
            int iSize;
            Reasoner r;
            Literal body;
            Event e;

            public RunnableImpl(Intention intention, Desire des, int size, Reasoner res, Literal b, Event ev)
            {
                Intention i = intention;
                Desire d = des;
                int iSize = size;
                Reasoner r = res;
                Literal body = b;
                Event e = ev;
            }

            public void Run()
            {
                r.RunAtBeginOfNextCycle(new RunnableImpl2(i, d, iSize, r, body, e));
                r.GetUserAgArch().WakeUpSense();
            }

            private class RunnableImpl2 : Runnable
            {
                Intention i;
                Desire d;
                int iSize;
                Reasoner r;
                Literal body;
                Event e;

                public RunnableImpl2(Intention intention, Desire des, int size, Reasoner res, Literal b, Event ev)
                {
                    Intention i = intention;
                    Desire d = des;
                    int iSize = size;
                    Reasoner r = res;
                    Literal body = b;
                    Event e = ev;
                }

                public void Run()
                {
                    bool drop = false;
                    if (i == null)
                    { // deadline in !!g, test if the agent still desires it
                        drop = d.AllDesires(r.GetCircumstance(), body, null, new Unifier()).Next();
                    }
                    else if (i.Size() >= iSize && i.HasTrigger(e.GetTrigger(), new Unifier()))
                    {
                        drop = true;
                    }
                    if (drop)
                    {
                        try
                        {
                            FailWithDeadline ia = new FailWithDeadline(i, e.getTrigger());
                            ia.FindGoalAndDrop(r, body, new Unifier());
                        }
                        catch (Exception e)
                        {
                            
                        }
                    }
                }
            }
        }
    }
}