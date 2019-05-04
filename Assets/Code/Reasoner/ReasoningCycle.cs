using Assets.Code.AsSemantics;
using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.Exceptions;
using Assets.Code.Stdlib;
using Assets.Code.Util;
using Assets.Code.Utilities;
using BDIMaAssets.Code.ReasoningCycle;
using BDIManager.Beliefs;
using BDIManager.Desires;
using BDIManager.Intentions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using static BDIManager.Desires.Desire;

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
    public class Reasoner
    {
        public enum State { StartRC, SelEv, RelPl, ApplPl, SelAppl, FindOp, AddIM, ProcAct, SelInt, ExecInt, ClrInt }


        private Agent ag = null;
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
        //private ConcurrentQueue<IRunnable> taskForBeginOfCycle = new ConcurrentQueue(); - I don't know how to use this
        private ConcurrentQueue<IRunnable> taskForBeginOfCycle = new ConcurrentQueue<IRunnable>();

        private Dictionary<Desire, ICircumstanceListener> listenersMap; //Map the circumstance listeners created for the goal listeners, used in remove goal listeners

        // the semantic rules are referred to in comments in the functions below
        private readonly string kqmlReceivedFunctor = Config.Get().GetKqmlFunctor();

        private static readonly Atom aNOCODE = new Atom("no_code");


        public Reasoner(Agent agent, Circumstance c, AgentArchitecture ar, Settings s)
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
            if (desireListeners == null)
            {
                desireListeners = new List<Desire>();
                listenersMap = new Dictionary<Desire, ICircumstanceListener>();
            } else
            {
                //To not instantiate two DesireListenerForMetaEvents
                foreach (Desire d in desireListeners)
                {
                    if (d is Desire)
                    {
                        return;
                    }
                }

                ICircumstanceListener cl = new CLImplementation(desire);
                circumstance.AddEventListener(cl);
                listenersMap.Add(desire, cl);
                desireListeners.Add(desire);
            }
        }

        public bool HasDesireListener()
        {
            return desireListeners != null && !desireListeners.Any();
        }

        public List<Desire> GetDesiresListeners()
        {
            return desireListeners;
        }

        public bool RemoveDesireListener(Desire desire)
        {
            ICircumstanceListener cl = listenersMap[desire];
            if (cl != null)
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
                agArch.ReasoningCycleStarting();
                circumstance.ResetSense();

                if (nrcslbr >= settings.Nrcbp())
                {
                    nrcslbr = 0;
                    //I don't know how to do this
                    /* 
                        synchronized (C.syncApPlanSense) {
                            ag.Buff(GetUserAgArch().Perceive());
                        }
                    */
                    agArch.CheckMail();
                }
                nrcslbr++; //Counting number of cycles since last belief revision

                //Produce sleep events
                if (CanSleep())
                {
                    if (!sleepingEvt)
                    {
                        if (ag.GetPL().GetCandidatePlans(PlanLibrary.TE_JAG_SLEEPING) != null)
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
                    if (circumstance.HasMsg())
                    {
                        sleepingEvt = false;
                    } else if (circumstance.HasEvent())
                    {
                        //Check if there is an event in C.E not produced by idle intention
                        foreach (Event e in circumstance.GetEvents())
                        {
                            Intention i = e.GetIntention();
                            if (!e.GetTrigger().Equals(PlanLibrary.TE_JAG_SLEEPING) || i != null && i.HasTrigger(PlanLibrary.TE_JAG_SLEEPING, new Unifier()))
                            {
                                sleepingEvt = false;
                                break;
                            }
                        }
                    }
                    if (!sleepingEvt && ag.GetPL().GetCandidatePlans(PlanLibrary.TE_JAG_AWAKING) != null)
                    {
                        circumstance.AddExternalEv(PlanLibrary.TE_JAG_AWAKING);
                    }
                }

                stepSense = State.StartRC;

                do
                {
                    ApplySemanticRuleSense();
                } while (stepSense != State.SelEv && agArch.IsRunning());
            } catch (Exception e)
            {
                //print(e.StackTrace());
                conf.circumstance.Create();
            }
        }

        public void Deliberate()
        {
            try
            {
                circumstance.ResetDeliberate();
                // run tasks allocated to be performed in the begin of the cycle
                IRunnable r;
                while (taskForBeginOfCycle.TryDequeue(out r))
                {
                    r.Run();
                }

                // IRunnable r = taskForBeginOfCycle.poll();
                // while(r != null) 
                // {
                // r.run(); //It is processed only things related to operations on goals/intentions resumed/suspended/finished It can be placed in the deliberate stage, but the problem is the sleep when the synchronous execution is adopted
                // r = taskForBeginOfCycle.poll();
                // }
                stepDeliberate = State.SelEv;
                do
                {
                    ApplySemanticRuleDeliberate();
                } while (stepDeliberate != State.ProcAct && agArch.IsRunning());
            }
            catch (Exception e)
            {
                //print(e.StackTrace());
                conf.circumstance.Create();
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
                } while (stepAct != State.StartRC && agArch.IsRunning());

                ExecuteAction action = circumstance.GetAction();
                if (action != null)
                {
                    circumstance.AddPendingAction(action);
                    // We need to send a wrapper for FA to the user so that add method then calls C.addFA (which control atomic things)
                    agArch.Act(action);
                }
            } catch (Exception e)
            {
                // print(e.StackTrace());
                conf.circumstance.Create();
            }
        }

        public bool CanSleep()
        {
            return (circumstance.IsAtomicIntentionSuspended() && !circumstance.HasFeedbackAction() && !conf.circumstance.HasMsg())  // atomic case
                  || (!conf.circumstance.HasEvent() &&    // other cases (deliberate)
                      !conf.circumstance.HasRunningIntention() && !conf.circumstance.HasFeedbackAction() && // (action)
                      !conf.circumstance.HasMsg() &&  // (sense)
                      taskForBeginOfCycle.IsEmpty &&
                      agArch.CanSleep());
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
                    ApplyClrInt(conf.GetCircumstance().GetSelectedIntention());
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
                if (!ip.IsFinished())
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
                if (!topTrigger.IsMetaEvent() && topTrigger.IsDesire() && HasDesireListener())
                {
                    foreach (Desire desire in desireListeners)
                    {
                        desire.DesireFinished(topTrigger, Desire.FinishStates.achieved);
                    }
                }

                if (ip.GetTrigger().IsDesire() && !ip.GetTrigger().IsAddition() && !i.IsFinished())
                {
                    ip = i.Peek();
                    if (ip.IsFinished() || !(ip.GetUnif().Unifies(ip.GetCurrentStep().GetBodyTerm(), topLiteral) && ip.GetCurrentStep().GetBodyType() == BodyType.achieve) ||
                        ip.GetCurrentStep().GetBodyTerm().GetType() == typeof(VarTerm))
                    {
                        ip.Pop();
                    }
                    while (!i.IsFinished() && !(ip.GetUnif().Unifies(ip.GetTrigger().GetLiteral(), topLiteral) && ip.GetTrigger().IsDesire())
                        && !(ip.GetUnif().Unifies(ip.GetCurrentStep().GetBodyTerm(), topLiteral) && ip.GetCurrentStep().GetBodyType() == BodyType.achieve)) {
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
                foreach (VarTerm var in ip.GetRenamedVars().GetFunction().Keys)
                {
                    UnnamedVar vt = (UnnamedVar)ip.GetRenamedVars().GetFunction()[var];
                    ip.GetUnif().Unifies(var, vt);
                    ITerm vl = values.GetFunction()[vt];
                    if (vl != null)
                    {
                        vl = vl.Capply(values);
                        if (vl.IsLiteral())
                        {
                            ((Literal)vl).MakeVarsAnnon();
                        }
                        ip.GetUnif().Bind(vt, vl);
                    }
                }
            }
        }

        private void ApplyExecInt()
        {
            confP.stepAct = State.ClrInt;

            Intention curInt = conf.GetCircumstance().GetSelectedIntention();
            if (curInt == null)
            {
                return;
            }
            if (curInt.IsFinished())
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
            IPlanBody h = ip.GetCurrentStep();
            ITerm bTerm = h.GetBodyTerm();

            if (bTerm.GetType() == typeof(VarTerm))
            {
                bTerm = bTerm.Capply(u);
                if (bTerm.IsVar())
                {
                    string msg = h.GetSrcInfo() + ": " + "Variable '" + bTerm + "' must be ground.";
                    if (!GenerateDesireDeletion(curInt, (List<ITerm>)JasonityException.CreateBasicErrorAnnots("body_var_without_value", msg)))
                    {
                        //logger.log(Level.SEVERE, msg);
                    }
                    return;
                }
                if (bTerm.IsPlanBody())
                {
                    if (h.GetBodyType() != BodyType.action)
                    {
                        string msg = h.GetSrcInfo() + ": " + "The operator '" + h.GetBodyType() + "' is lost with the variable '" + bTerm + "' unified with a plan body. ";
                        if (!GenerateDesireDeletion(curInt, (List<ITerm>)JasonityException.CreateBasicErrorAnnots("body_var_with_op", msg)))
                        {
                            //logger.log(Level.SEVERE, msg);
                        }
                        return;
                    }
                }
            }

            if (bTerm.IsPlanBody())
            {
                h = (IPlanBody)bTerm;
                if (h.GetPlanSize() > 1)
                {
                    h = (IPlanBody)bTerm.Clone();
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

            if (h.GetBodyType() == BodyType.none)
            {

            } else if (h.GetBodyType() == BodyType.action) {
                body = (Literal)body.Capply(u);
                confP.GetCircumstance().SetAction(new ExecuteAction(body, curInt));
            } else if (h.GetBodyType() == BodyType.internalAction) {

                bool ok = false;
                List<ITerm> errorAnnots = null;
                try
                {
                    InternalAction ia = ((InternalActionLiteral)bTerm).GetIA(ag);
                    ITerm[] terms = ia.PrepareArguments(body, u);
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
                            errorAnnots = (List<ITerm>)JasonityException.CreateBasicErrorAnnots("ia_failed", "");
                        }
                    }
                    if (ok && ia.SuspendIntention())
                    {
                        UpdateIntention(curInt);
                    }
                }
                catch (NoValueException e)
                {
                    string msg = e.Message + " Ungrounded variables = [";
                    string v = "";
                    foreach (VarTerm var in body.GetSingletonVars())
                    {
                        if (u.Get(var) == null)
                        {
                            msg += v + var;
                            v = ",";
                        }
                    }
                    msg += "].";
                    e = new NoValueException(msg);
                    errorAnnots = (List<ITerm>)e.GetErrorTerms();
                    if (!GenerateDesireDeletion(curInt, errorAnnots))
                    {
                        //logger.log(Level.SEVERE, body.getErrorMsg() + ": " + e.getMessage());
                    }

                    ok = true;

                }
                catch (JasonityException e)
                {
                    errorAnnots = (List<ITerm>)e.GetErrorTerms();
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
            } else if (h.GetBodyType() == BodyType.constraint)
            {

                IEnumerator<Unifier> iu = (((ILogicalFormula)bTerm).LogicalConsequence(ag, u));
                if (iu.MoveNext())
                {
                    ip.SetUnif(iu.Current);
                    UpdateIntention(curInt);
                } else
                {
                    string msg = "Constraint " + h + " was not satisfied (" + h.GetSrcInfo() + ") un=" + u;
                    GenerateDesireDeletion(curInt, (List<ITerm>)JasonityException.CreateBasicErrorAnnots(new Atom("constraint_failed"), msg));
                    //logger.fine(msg);
                }
            } else if (h.GetBodyType() == BodyType.achieve)
            {
                body = PrepareBodyForEvent(body, u, curInt.Peek());
                Event evt = conf.GetCircumstance().AddAchieveDesire(body, curInt);
                confP.stepAct = State.StartRC;
                CheckHardDeadline(evt);

            } else if (h.GetBodyType() == BodyType.achieveNF)
            {
                body = PrepareBodyForEvent(body, u, null);
                Event evt = conf.GetCircumstance().AddAchieveDesire(body, Intention.emptyInt);
                CheckHardDeadline(evt);
                UpdateIntention(curInt);
            } else if (h.GetBodyType() == BodyType.test)
            {
                ILogicalFormula f = (ILogicalFormula)bTerm;
                if (conf.GetAgent().Believes(f, u))
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
                            Event evt = new Event(t, curInt);
                            if (ag.GetPL().HasCandidatePlan(t))
                            {
                                //if (logger.isLoggable(Level.FINE)) 
                                //{
                                //   logger.fine("Test Goal '" + bTerm + "' failed as simple query. Generating internal event for it: " + te);
                                //}
                                conf.GetCircumstance().AddEvent(evt);
                                confP.stepAct = State.StartRC;
                                fail = false;
                            }
                        }
                    }
                    if (fail)
                    {
                        // if (logger.isLoggable(Level.FINE))
                        // {
                        //    logger.fine("Test '" + bTerm + "' failed (" + h.getSrcInfo() + ").");
                        //  }
                        GenerateDesireDeletion(curInt, (List<ITerm>)JasonityException.CreateBasicErrorAnnots("test_goal_failed", "Failed to test '" + bTerm + "'"));
                    }
                }
            } else if (h.GetBodyType() == BodyType.delAddBel) {
                Literal b2 = PrepareBodyForEvent(body, u, curInt.Peek());
                b2.MakeTermsAnnon();
                try
                {
                    List<Literal>[] result = ag.Brf(null, b2, curInt);
                    if (result != null)
                    {
                        UpdateEvents(result, Intention.emptyInt);
                    }
                } catch (RevisionFailedException re)
                {
                    GenerateDesireDeletion(curInt, (List<ITerm>)JasonityException.CreateBasicErrorAnnots("belief_revision_failed", "BRF failed for '" + body + "'"));
                }
            } else if (h.GetBodyType() == BodyType.addBel || h.GetBodyType() == BodyType.addBelBegin || h.GetBodyType() == BodyType.addBelEnd || h.GetBodyType() == BodyType.addBelNewFocus)
            {
                Intention newFocus = Intention.emptyInt;
                bool isSameFocus = GetSettings().SameFocus() && h.GetBodyType() != BodyType.addBelNewFocus;
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
                    if (h.GetBodyType() == BodyType.addBelEnd)
                    {
                        result = GetAgent().Brf(body, null, curInt, true);
                    }
                    else
                    {
                        result = GetAgent().Brf(body, null, curInt);
                    }
                    if (result != null)
                    {
                        UpdateEvents(result, newFocus);
                        if (!isSameFocus)
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

            } else if (h.GetBodyType() == BodyType.delBelNewFocus || h.GetBodyType() == BodyType.delBel) {
                Intention newFocus = Intention.emptyInt; 
                bool isSameFocus = GetSettings().SameFocus() && h.GetBodyType() != BodyType.delBelNewFocus;
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
                    List<Literal>[] result = GetAgent().Brf(null, body, curInt);
                    if (result != null)
                    {
                        UpdateEvents(result, newFocus);
                        if (!isSameFocus)
                        {
                            UpdateIntention(curInt);
                        }
                    } else {
                        UpdateIntention(curInt);
                    }
                } catch (RevisionFailedException re)
                {
                    GenerateDesireDeletion(curInt, null);
                }
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
            if (ev.IsInternal() || GetCircumstance().HasListener() || ag.GetPL().HasCandidatePlan(ev.GetTrigger()))
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
                    Dictionary<VarTerm, ITerm> adds = null;
                    foreach (VarTerm v in renamedVars)
                    {
                        ITerm t = u.GetFunction()[v];
                        if (t != null && t.IsVar())
                        {
                            if (adds == null)
                                adds = new Dictionary<VarTerm, ITerm>();
                            try
                            {
                                adds.Add((VarTerm)t, renamedVars.GetFunction()[v]);
                            } catch (Exception e)
                            {
                                //logger.log(Level.SEVERE, "*** Error adding var into renamed vars. var=" + v + ", value=" + t + ".", e);
                            }
                        }
                    }
                    if (adds != null)
                    {
                        renamedVars.GetFunction().Concat(adds);
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
            Literal hdl = body.GetAnnot(AsSyntax.AsSyntax.GetHardDeadLineStr());
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
                deadline = (int)((INumberTerm)hdl.GetTerm(0)).Solve();
            }
            catch (NoValueException e)
            {

            }

            Agent.GetExecutor().AddTask(new RunnableImpl(i, iSize, this, body, evt));

            //Agent.GetScheduler().Schedule(new RunnableImpl(), deadline, TimeUnit.MILLISECONDS);

        }

        public bool GenerateDesireDeletion(Intention i, List<ITerm> errorAnnots)
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

            ITerm bodyPart = ip.GetCurrentStep().GetBodyTerm().Capply(ip.GetUnif());
            SetDefaultFailureAnnots(failEvent, bodyPart, errorAnnots);
            if (ip.IsDesireAdd())
            {
                if (HasDesireListener())
                {
                    foreach (Desire desire in desireListeners)
                    {
                        desire.DesireFailed(ip.GetTrigger());
                        if (!failEventIsRelevant)
                        {
                            desire.DesireFinished(ip.GetTrigger(), Desire.FinishStates.unachieved);
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

        private static void SetDefaultFailureAnnots(Event failEvent, ITerm body, List<ITerm> errorAnnots)
        {
            if (errorAnnots == null)
            {
                errorAnnots = (List<ITerm>)JasonityException.CreateBasicErrorAnnots(JasonityException.UNKNOWN_ERROR, "");
            }

            Literal eLiteral = failEvent.GetTrigger().GetLiteral().ForceFullLiteralImpl();
            eLiteral.AddAnnots(errorAnnots);

            Literal bodyTerm = aNOCODE;
            ITerm codesrc = aNOCODE;
            ITerm codeline = aNOCODE;
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
                eLiteral.AddAnnot(AsSyntax.AsSyntax.CreateStructure("code", bodyTerm.Copy().MakeVarsAnnon()));
            }

            if (eLiteral.GetAnnot("code_src") == null)
            {
                eLiteral.AddAnnot(AsSyntax.AsSyntax.CreateStructure("code_src", codesrc));
            }

            if(eLiteral.GetAnnot("code_line") == null)
            {
                eLiteral.AddAnnot(AsSyntax.AsSyntax.CreateStructure("code_line", codeline));
            }
        }

        public Event FindEventForFailure(Intention i, Trigger trigger)
        {
            if (i != Intention.emptyInt)
            {
                return i.FindEventForFailure(trigger, GetAgent().GetPL(), GetCircumstance()).Key;
            } else if (trigger.IsDesire() && trigger.IsAddition())
            {
                Trigger failTrigger = new Trigger(TEOperator.del, trigger.GetTEType(), trigger.GetLiteral());
                if(GetAgent().GetPL().HasCandidatePlan(failTrigger))
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

            confP.GetCircumstance().SetSelectedIntention(GetCircumstance().RemoveAtomicIntention());
            if (confP.GetCircumstance().GetSelectedIntention() != null)
            {
                return;
            }

            if (!conf.GetCircumstance().IsAtomicIntentionSuspended() && conf.GetCircumstance().HasRunningIntention())
            {
                confP.GetCircumstance().SetSelectedIntention(conf.GetAgent().SelectIntention(conf.GetCircumstance().GetRunningIntentions()));
                //if (logger.isLoggable(Level.FINE)) logger.fine("Selected intention " + confP.C.SI);
                if (confP.GetCircumstance().GetSelectedIntention() != null)
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
                
                    if (GetCircumstance().RemovePendingIntention(curInt.GetID().ToString()) != null) {
                        if (a.GetResult())
                        {
                            UpdateIntention(curInt);
                            ApplyClrInt(curInt);

                            if (HasDesireListener())
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
                            IListTerm annots = JasonityException.CreateBasicErrorAnnots("action_failed", reason);
                            if (a.GetFailureReason() != null)
                            {
                                annots.Append(a.GetFailureReason());
                            }
                            GenerateDesireDeletion(curInt, (List<ITerm>)annots);
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
            IntendedPlan ip = new IntendedPlan(conf.GetCircumstance().GetSelectedOption(), conf.GetCircumstance().GetSelectedEvent().GetTrigger());

            //Rule ExtEv
            if (conf.GetCircumstance().GetSelectedEvent().GetIntention() == Intention.emptyInt)
            {
                Intention intention = new Intention();
                intention.Push(ip);
                confP.GetCircumstance().AddRunningIntention(intention);
            } else
            {
                //Rule IntEv
                if(GetSettings().IsTROon())
                {
                    IntendedPlan top = confP.GetCircumstance().GetSelectedEvent().GetIntention().Peek();

                    if(top != null && top.GetTrigger().IsAddition() && ip.GetTrigger().IsAddition() && 
                        top.GetTrigger().IsDesire() && ip.GetTrigger().IsDesire() &&
                        top.GetCurrentStep().GetBodyNext() == null &&
                        top.GetTrigger().GetLiteral().GetPredicateIndicator().Equals(ip.GetTrigger().GetLiteral().GetPredicateIndicator()))
                    {
                        confP.GetCircumstance().GetSelectedEvent().GetIntention().Pop();

                        IntendedPlan ipBase = confP.GetCircumstance().GetSelectedEvent().GetIntention().Peek();
                        if (ipBase != null && ipBase.GetRenamedVars() != null)
                        {
                            foreach (VarTerm var in ipBase.GetRenamedVars())
                            {
                                VarTerm vl = (VarTerm)ipBase.GetRenamedVars().GetFunction()[var];
                                ITerm t = top.GetUnif().Get(vl);
                                if (t != null)
                                {
                                    if (t.GetType() == typeof(Literal))
                                    {
                                        Literal l = (Literal)t.Capply(top.GetUnif());
                                        l.MakeVarsAnnon(top.GetRenamedVars());
                                        ip.GetUnif().GetFunction().Add(vl, l);
                                    } else
                                    {
                                        ip.GetUnif().GetFunction().Add(vl, t);
                                    }
                                }
                            }
                        }
                    }
                }
                confP.GetCircumstance().GetSelectedEvent().GetIntention().Push(ip);
                confP.GetCircumstance().AddRunningIntention(confP.GetCircumstance().GetSelectedEvent().GetIntention());
            }
            confP.stepDeliberate = State.ProcAct;
        }

        private void ApplyFindOp()
        {
            confP.stepDeliberate = State.AddIM;

            List<Plan> candidateRPs = conf.GetAgent().GetPL().GetCandidatePlans(conf.GetCircumstance().GetSelectedEvent().GetTrigger());
            if (candidateRPs != null)
            {
                foreach (Plan p in candidateRPs)
                {
                    Unifier relUn = p.IsRelevant(conf.GetCircumstance().GetSelectedEvent().GetTrigger());
                    if (relUn != null)
                    {
                        ILogicalFormula context = p.GetContext();
                        if (context != null)
                        {
                            confP.GetCircumstance().SetSelectedOption(new Option(p, relUn));
                            return;
                        } else
                        {
                            IEnumerator<Unifier> r = context.LogicalConsequence(ag, relUn);
                            if (r != null && r.MoveNext())
                            {
                                confP.GetCircumstance().SetSelectedOption(new Option(p, r.Current));
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
            confP.GetCircumstance().SetSelectedOption(conf.GetAgent().HasCustomSelectOption(confP.GetCircumstance().GetApplicablePlans()));

            if (confP.GetCircumstance().GetSelectedOption() != null)
            {
                confP.stepDeliberate = State.AddIM;
                //if (logger.isLoggable(Level.FINE)) logger.fine("Selected option "+confP.C.SO+" for event "+confP.C.SE);
            } else
            {
                //logger.fine("** selectOption returned null!");
                GenerateDesireDeletionFromEvent((List<ITerm>)JasonityException.CreateBasicErrorAnnots("no_option", "selectOption returned null"));
                confP.stepDeliberate = State.ProcAct;
            }
        }

        private void ApplyApplPl()
        {
            confP.GetCircumstance().SetApplicablePlans(ApplicablePlans(confP.GetCircumstance().GetRelevantPlans()));

            // Rule Appl1
            if (confP.GetCircumstance().GetApplicablePlans() != null || GetSettings().Retrieve())
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
                    ILogicalFormula context = o.GetPlan().GetContext();
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
                        IEnumerator<Unifier> r = context.LogicalConsequence(ag, o.GetUnifier());
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
                                    o = new Option(o.GetPlan(), null); //I need to create a new option for the next loop step
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
            confP.GetCircumstance().SetRelevantPlans(RelevantPlans(conf.GetCircumstance().GetSelectedEvent().GetTrigger()));

            //Rule Rel1
            if (confP.GetCircumstance().GetRelevantPlans() != null || GetSettings().Retrieve())
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
            if (conf.GetCircumstance().GetSelectedEvent().GetTrigger().IsDesire() && !conf.GetCircumstance().GetSelectedEvent().GetTrigger().IsMetaEvent())
            {
                //Can't carry on, no relevant/applicable plan
                try
                {
                    if (conf.GetCircumstance().GetSelectedEvent().GetIntention() != null && conf.GetCircumstance().GetSelectedEvent().GetIntention().Size() > 3000) //I don't know why is 3000
                    {
                        //logger.warning("we are likely in a problem with event " + conf.C.SE.getTrigger() + " the intention stack has already " + conf.C.SE.getIntention().size() + " intended means!");
                    }
                    string msg = "Found a goal for which there is no " + m + " plan:" + conf.GetCircumstance().GetSelectedEvent().GetTrigger();
                    if (!GenerateDesireDeletionFromEvent((List<ITerm>)JasonityException.CreateBasicErrorAnnots("no_" + m, msg)))
                    {
                        //logger.warning(msg);
                    }
                } catch (Exception e)
                {
                    return;
                }
            } else if (conf.GetCircumstance().GetSelectedEvent().IsInternal())
            {
                // e.g. belief addition as internal event, just go ahead
                // but note that the event was relevant, yet it is possible
                // the programmer just wanted to add the belief and it was
                // relevant by chance, so just carry on instead of dropping the
                // intention
                Intention i = conf.GetCircumstance().GetSelectedEvent().GetIntention();
                JoinRenamedVarsIntoIntentionUnifier(i.Peek(), i.Peek().GetUnif());
                UpdateIntention(i);
            } else if (GetSettings().Requeue())
            {
                confP.GetCircumstance().AddEvent(conf.GetCircumstance().GetSelectedEvent());
            } else
            {
                confP.stepDeliberate = State.SelEv;
            }
        }

        private bool GenerateDesireDeletionFromEvent(List<ITerm> failAnnots)
        {
            Event e = conf.GetCircumstance().GetSelectedEvent();
            if (e == null)
            {
                //logger.warning("** It was impossible to generate a goal deletion event because SE is null! " + conf.C);
                return false;
            }

            Trigger t = e.GetTrigger();
            bool failEventGenerated = false;
            if(t.IsAddition() && t.IsDesire())
            {
                if (HasDesireListener())
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
            List<Plan> candidateRP = conf.GetAgent().GetPL().GetCandidatePlans(t);

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
            confP.GetCircumstance().SetSelectedEvent(GetCircumstance().RemoveAtomicEvent());
            if(confP.GetCircumstance().GetSelectedEvent() != null)
            {
                confP.stepDeliberate = State.RelPl;
                return;
            }

            if (conf.GetCircumstance().HasEvent())
            {
                //Rule SelEv1
                confP.GetCircumstance().SetSelectedEvent(conf.GetAgent().SelectEvent(confP.GetCircumstance().GetEvents()));
                if (confP.GetCircumstance().GetSelectedEvent() != null)
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
                ITerm content = null;
                if (m.GetPropCont().GetType() == typeof(ITerm))
                {
                    content = (ITerm)m.GetPropCont();
                } else
                {
                    try
                    {
                        content = AsSyntax.AsSyntax.ParseTerm(m.GetPropCont().ToString()); 
                    } catch (Exception e)
                    {
                        content = new ObjectTermImpl(m.GetPropCont());
                    }
                }

                //Checks if an intention was suspended waiting this message
                Intention intention = null;
                if (m.GetInReplyTo() != null)
                {
                    intention = GetCircumstance().GetPendingIntentions()[m.GetInReplyTo()];
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
                        content = AddNestedSourceStdLib.AddAnnotToList(content, new AsSyntax.Atom(m.GetSender()));
                    } else if (send.GetTerm(1).ToString().Equals("askAll") && content.IsList()) //Adds source in each answer if possible
                    {
                        IListTerm tail = new ListTermImpl();
                        foreach (ITerm t in ((IListTerm)content))
                        {
                            ITerm term = AddNestedSourceStdLib.AddAnnotToList(t, new Atom(m.GetSender()));
                            tail.Append(term);
                        }
                        content = tail;
                    }

                    //Test the case of sync ask with many receivers
                    Unifier un = intention.Peek().GetUnif();
                    ITerm rec = send.GetTerm(0).Capply(un);
                    if (rec.IsList()) //Send to many receivers
                    {
                        //Put the answers in the unifier
                        VarTerm answers = new VarTerm("AnsList___" + m.GetInReplyTo());
                        IListTerm listOfAnswers = (IListTerm)un.Get(answers);
                        if (listOfAnswers == null)
                        {
                            listOfAnswers = new ListTermImpl();
                            un.Unifies(answers, listOfAnswers);
                        }
                        listOfAnswers.Append(content);
                        int nbReceivers = ((IListTerm)send.GetTerm(0)).Size();
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
                        string sender = m.GetSender();
                        if (sender.Equals(GetUserAgArch().GetAgentName()))
                        {
                            sender = "self";
                        }

                        bool added = false;
                        if (!settings.IsSync() && !ag.GetPL().HasUserKqmlReceivedPlans() && content.IsLiteral() && !content.IsList())
                        {
                            //Optimisation to jump kqmlPlans
                            if (m.GetIlForce().Equals("achieve"))
                            {
                                content = AddNestedSourceStdLib.AddAnnotToList(content, new Atom(sender));
                                GetCircumstance().AddEvent(new Event(new Trigger(TEOperator.add, TEType.achieve, (Literal)content), Intention.emptyInt));
                                added = true;
                            } else if (m.GetIlForce().Equals("tell"))
                            {
                                content = AddNestedSourceStdLib.AddAnnotToList(content, new Atom(sender));
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

                            UpdateEvents(new Event(new Trigger(TEOperator.add, TEType.achieve, received), Intention.emptyInt));
                        }
                    } else
                    {
                        //logger.fine("Ignoring message "+m+" because it is received after the timeout.");
                    }
                }
            }
        }

        private void ResumeSyncAskIntention(string msgId, ITerm answerVar, ITerm answerValue)
        {
            Intention i = GetCircumstance().RemovePendingIntention(msgId);
            i.Peek().RemoveCurrentStep();
            if (i.Peek().GetUnif().Unifies(answerVar, answerValue))
            {
                GetCircumstance().ResumeIntention(i);
            } else
            {
                GenerateDesireDeletion(i, (List<ITerm>)JasonityException.CreateBasicErrorAnnots("ask_failed", "reply of an ask message ('"+answerValue+"') does not unify with fourth argument of .send ('"+answerVar+"')"));
            }
        }

        private void GenerateDesireDeletion(Intention i)
        {
            throw new NotImplementedException();
        }

        public Agent GetAgent()
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

        public void RunAtBeginOfNextCycle(IRunnable r)
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
            return "Reasoning cycle of agent " + GetUserAgArch().GetAgentName();
        }

        class FailWithDeadline : FailDesireStdLib
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
                        if (r.HasDesireListener())
                        {
                            foreach (Desire d in r.GetDesiresListeners())
                            {
                                d.DesireFailed(te);
                            }
                        }

                        Event failEvent = r.FindEventForFailure(i, te);
                        if (failEvent != null)
                        {
                            failEvent.GetTrigger().GetLiteral().AddAnnot(JasonityException.CreateBasicErrorAnnots("deadline_reached", ""));
                            r.GetCircumstance().AddEvent(failEvent);
                            //ts.getLogger().fine("'hard_deadline(" + g + ")' is generating a goal deletion event: " + failEvent.getTrigger());
                            return 2;
                        } else
                        {
                            //ts.getLogger().fine("'hard_deadline(" + g + ")' is removing the intention without event:\n" + i);
                            return 3;
                        }
                    }
                }
                return 0;
            }

           
        }

        /*
         This innner class is here to imitate the anonymous interface implementation that exist on Java but not here
         */

        private class CLImplementation : ICircumstanceListener
        {
            private Desire d;

            public CLImplementation(Desire desire)
            {
                d = desire;
            }

            public void EventAdded(Event e)
            {
                if (e.GetTrigger().IsAddition() && e.GetTrigger().IsDesire())
                    d.DesireStarted(e);
            }

            public void IntentionAdded(Intention i)
            {
                
            }

            public void IntentionDropped(Intention i)
            {
                foreach (IntendedPlan ip in i.GetIntendedPlan())
                {
                    if (ip.GetTrigger().IsAddition() && ip.GetTrigger().IsDesire())
                        d.DesireFinished(ip.GetTrigger(), FinishStates.dropped);
                }
            }

            public void IntentionResumed(Intention i)
            {
                foreach (IntendedPlan ip in i.GetIntendedPlan())
                {
                    if (ip.GetTrigger().IsAddition() && ip.GetTrigger().IsDesire())
                        d.DesireResumed(ip.GetTrigger());
                }
            }

            public void IntentionSuspended(Intention i, string reason)
            {
                foreach (IntendedPlan ip in i.GetIntendedPlan())
                {
                    if (ip.GetTrigger().IsAddition() && ip.GetTrigger().IsDesire())
                        d.DesireSuspended(ip.GetTrigger(), reason);
                }
            }
        }

        private class RunnableImpl : IRunnable
        {
            Intention i;
            int iSize;
            Reasoner r;
            Literal body;
            Event e;

            public RunnableImpl(Intention intention, int size, Reasoner res, Literal b, Event ev)
            {
                Intention i = intention;
                int iSize = size;
                Reasoner r = res;
                Literal body = b;
                Event e = ev;
            }

            public void Run()
            {
                r.RunAtBeginOfNextCycle(new RunnableImpl2(i, iSize, r, body, e));
                r.GetUserAgArch().WakeUpSense();
            }

            private class RunnableImpl2 : IRunnable
            {
                Intention i;
                int iSize;
                Reasoner r;
                Literal body;
                Event e;

                public RunnableImpl2(Intention intention, int size, Reasoner res, Literal b, Event ev)
                {
                    i = intention;
                    iSize = size;
                    r = res;
                    body = b;
                    e = ev;
                }

                public void Run()
                {
                    bool drop = false;
                    if (i == null)
                    { // deadline in !!g, test if the agent still desires it
                        drop = DesireStdLib.AllDesires(r.GetCircumstance(), body, null, new Unifier()).MoveNext();
                    }
                    else if (i.Size() >= iSize && i.HasTrigger(e.GetTrigger(), new Unifier()))
                    {
                        drop = true;
                    }
                    if (drop)
                    {
                        try
                        {
                            FailWithDeadline ia = new FailWithDeadline(i, e.GetTrigger());
                            ia.FindDesireAndDrop(r, body, new Unifier());
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
