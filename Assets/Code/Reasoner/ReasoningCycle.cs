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
using Assets.Code.BDIManager.Intentions;

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

        //private TransitionSystem confP;
        //private TransitionSystem conf;

        //private ConcurrentQueue taskForBeginOfCycle = new ConcurrentQueue(); - I don't know how to use this

        private Dictionary<Desire, CircumstanceListener> listenersMap; //Map the circumstance listeners created for the goal listeners, used in remove goal listeners


        public Reasoner(Assets.Code.Agent.Agent agent, Circumstance c, AgentArchitecture ar, Settings s)
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

                CircumstanceListener cl = new CircumstanceListenerImplementation();
                /*CircumstanceListener cl = new CircumstanceListener() {

                    public void intentionDropped(Intention i) {
                        for (IntendedMeans im: i) //.getIMs())
                            if (im.getTrigger().isAddition() && im.getTrigger().isGoal())
                                gl.goalFinished(im.getTrigger(), FinishStates.dropped);
                    }

                    public void intentionSuspended(Intention i, String reason) {
                        for (IntendedMeans im: i) //.getIMs())
                            if (im.getTrigger().isAddition() && im.getTrigger().isGoal())
                                gl.goalSuspended(im.getTrigger(), reason);
                    }

                    public void intentionResumed(Intention i) {
                        for (IntendedMeans im: i) //.getIMs())
                            if (im.getTrigger().isAddition() && im.getTrigger().isGoal())
                                gl.goalResumed(im.getTrigger());
                    }

                    public void eventAdded(Event e) {
                        if (e.getTrigger().isAddition() && e.getTrigger().isGoal())
                            gl.goalStarted(e);
                    }

                    public void intentionAdded(Intention i) {  }
                };*/
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
                //conf.C.Create();
            }
        } 

        public void Deliberate()
        {
            try
            {
                circumstance.ResetDeliberate();
                // run tasks allocated to be performed in the begin of the cycle
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
                //conf.C.Create();
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
                // conf.C.Create();
            }
        }

        public bool CanSleep()
        {
            return true;
        }

        public void ApplySemanticRuleSense()
        {

        }

        public void ApplySemanticRuleDeliberate()
        {

        }

        public void ApplySemanticRuleAct()
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
    }
}
