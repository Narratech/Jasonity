using Assets.Code.Agent;
using BDIManager.Desires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using BDIManager.Intentions;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using BDIMaAssets.Code.ReasoningCycle;
using Assets.Code.Utilities;

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

        private void ApplyClrInt(Intention i)
        {
            
        }

        private void JoinRenamedVarsIntoIntentionUnifier(IntendedPlan ip, Unifier values)
        {
            
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
                    break;

            }
        }

        private void UpdateIntention(Intention curInt)
        {
            throw new NotImplementedException();
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

        private object ApplicablePlans(object v)
        {
            throw new NotImplementedException();
        }

        private void ApplyRelPl()
        {
            //Get all relevant plans for the selected event
            confP.GetCircumstance().SetRP(relevantPlans(conf.GetCircumstance().GetSE().GetTrigger()));

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

        private bool generateGoalDeletionFromEvent()
        {
            throw new NotImplementedException();
        }

        private List<Option> relevantPlans(Trigger trigger)
        {
            throw new NotImplementedException();
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
    }
}
