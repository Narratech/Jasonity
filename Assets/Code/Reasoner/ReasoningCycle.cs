using Assets.Code.Agent;
using BDIManager.Desires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public enum State { StartRC, SelEv, RelPl, ApplPl, SelAppl, FindOp, AddIM, ProAct, SelInt, ExecInt, ClrInt }


        private Assets.Code.Agent.Agent ag = null; 
        private AgentArchitecture agArch = null;
        private Settings settings = null;
        private Circumstance circumstance = null;

        private State stepSense = State.StartRC;
        private State stepDeliberate = State.SelEv;
        private State stepAct = State.ProAct;

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
                    if(circumstance.HashMsg())
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
                } while (stepDeliberate != State.ProAct && GetUserAgArch().IsRunning());
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
                stepAct = State.ProAct;
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
                    ApplyAddIM();
                    break;
                default:
                    break;
            }
        }

        public void ApplySemanticRuleAct()
        {
            switch (stepAct)
            {
                case State.ProAct:
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
            while(true) //Quit the method by return
            {
                //Rule ClrInt
                if (i == null)
                    return;
                if (i.IsFinished())
                {
                    //Intention finished, remove it
                    confP.GetCircumstance().DropRunningIntention(i);
                    return;
                }

                IntendedPlan ip = i.Peek();
                if (!ip.IsFinished())
                {
                    //Nothing to do
                    return;
                }

                IntendedPlan topIP = i.Pop();
                Trigger topTrigger = topIP.GetTrigger();
                Literal topLiteral = topTrigger.GetLiteral();

                //produce ^!g[state(finished)[reason(achieved)]] event
                if (!topTrigger.IsMetaEvent() && topTrigger.IsGoal() && HasGoalListener())
                {
                    foreach (Desire desire in desireListeners)
                    {
                        desire.GoalFinished(topTrigger, FinishStates.achieved);
                    }
                }

                //if hash finished a failure handling IP ...
                if (ip.GetTrigger().IsGoal() && !ip.GetTrigger().IsAddition() && !i.IsFinished())
                {
                    ip = i.Peek();
                    if (ip.IsFinished() || !(ip.GetUnify().Unifies(ip.GetCurrentStep().GetBodyTerm(), topLiteral) && ip.GetCurrentStep().GetBodyType() == PlanBody.BodyType.achieve)
                        || ip.GetCurrentStep().GetBodyTerm().GetType() == typeof(VarTerm))
                    {
                        ip = i.Pop();
                    }
                    while(!i.IsFinished() &&
                        !(ip.GetUnify().Unifies(ip.GetTrigger().GetLiteral(), topLiteral) && ip.GetTrigger().IsGoal()) &&
                        !(ip.GetUnify().Unifies(ip.GetCurrentStep().GetBodyTerm(), topLiteral) && ip.GetCurrentStep().GetBodyType() == PlanBody.BodyType.achieve))
                    {
                        ip = i.Pop();
                    }
                } 

                if(!i.IsFinished())
                {
                    ip = i.Peek();
                    if(!ip.IsFinished())
                    {
                        JoinRenamedVarsIntoIntentionUnifier(ip, topIP.GetUnifier());
                        ip.RemoveCurrentStep();
                    }
                }
            }
        }

        private void JoinRenamedVarsIntoIntentionUnifier(IntendedPlan ip, object p)
        {
            throw new NotImplementedException();
        }

        private void ApplyExecInt()
        {
            confP.stepAct = State.ClrInt; //default next step
            Intention curInt = conf.GetCircumstance().GetSI();
            if(curInt == null)
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
                //For empty plans! may need unif, etc
                UpdateIntention(curInt);
                return;
            }
            Unifier u = ip.GetUnifier();
            PlanBody h = ip.GetCurrentStep();

            Term bTerm = h.GetBodyTerm();

            if(bTerm == typeof(VarTerm))
            {
                bTerm = bTerm.Capply(u);
                if(bTerm.IsVar()) //The case of !A with A not ground
                {
                    return;
                }
                if (bTerm.IsPlanBody())
                {
                    if(h.GetBodyType() != PlanBody.BodyType.action)
                    {
                        return;
                    }
                }
            }

            if (bTerm.IsPlanBody())
            {
                h = (PlanBody)bTerm;
                if(bTerm.IsPlanBody())
                {
                    h = (PlanBody)bTerm;
                    if(h.GetPlanSize() > 1)
                    {
                        h = (PlanBody)bTerm.Clone();
                        h.Add(ip.GetCurrentStep().GetBodyNext());
                        ip.InsertAsNextStep(h.GetBodyNext());
                    }
                    bTerm = h.GetBodyTerm();
                }
            }

            Literal body = null;
            if(bTerm.GetType() == typeof(Literal))
            {
                body = (Literal)bTerm;
            }

            switch (h.getBodyType())
            {
                case PlanBody.BodyType.none:
                    break;
                //Rule action
                case PlanBody.BodyType.action:
                    body = (Literal)body.Capply(u);
                    confP.GetCircumstance().A = new ExecuteAction(body, curInt);
                    break;
                case PlanBody.BodyType.internalAction:
                    
                    break;
                case PlanBody.BodyType.constraint:
                    break;
                    //Rule achieve
                case PlanBody.BodyType.achieve:
                    break;
                //Rule achieve as a new focus (!! operator)
                case PlanBody.BodyType.achieveNF:
                    break;
                //Rule test
                case PlanBody.BodyType.test:
                    break;
                case PlanBody.BodyType.delAddBel:
                    break;
                //Add the belief, no breaks
                case PlanBody.BodyType.addBel:
                case PlanBody.BodyType.addBelBegin:
                case PlanBody.BodyType.addBelEnd:
                case PlanBody.BodyType.addBelNewFocus:
                    break;
                case PlanBody.BodyType.delBelNewFocus:
                case PlanBody.BodyType.delBel:
                    break;
            }
        }

        private void UpdateIntention(Intention curInt)
        {
            throw new NotImplementedException();
        }

        private void ApplySelInt()
        {

        }

        private void ApplyProcAct()
        {

        }

        private void ApplyAddIM()
        {

        }

        private void ApplyFindOp()
        {

        }

        private void ApplySelAppl()
        {

        }

        private void ApplyApplPl()
        {

        }

        private void ApplyRelPl()
        {

        }

        private void ApplySelEv()
        {

        }

        private void ApplyProcMsg()
        {

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
                    d.GoalStarted(e);
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
                        d.GoalResumed(ip.GetTrigger());
                }
            }

            public void IntentionSuspended(Intention i, string reason)
            {
                foreach (IntendedPlan ip in i.GetIntendedPlan())
                {
                    if (ip.GetTrigger().IsAddition() && ip.GetTrigger().IsGoal())
                        d.GoalSuspended(ip.GetTrigger(), reason);
                }
            }
        }
    }
}
