using Assets.Code.Agent;
using BDIManager.Desires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Assets.Code.BDIManager.Desires;
using BDIManager.Intentions;

/*
 * Implements the reasoning cycle
 * */

namespace Assets.Code.BDIManager
{
    class Reasoner
    {
        public enum State { StartRC, SelEv, RelPl, ApplPl, SelAppl, FindOp, AddIM, ProAct, SelInt, ExecInt, ClrInt }
        private const int DEFAULT_NUMBER_REASONING_CYCLES = 1;


        private Assets.Code.Agent.Agent ag = null; 
        private AgentArchitecture agArch = null;

        private Circumstance C = null;

        private State stepSense = State.StartRC;
        private State stepDeliberate = State.SelEv;
        private State stepAct = State.ProAct;

        private List<Desire> desireListeners;

        private bool sleepingEvt = false;
 

        private int nrcslbr = 1; //Number of reasoning cycles since last belief revision. This maybe has to be in a Settings class

        //private TransitionSystem confP;
        //private TransitionSystem conf;

        //private Queue<Runnable> taskForBeginOfCycle = new ConcurrentLinkedQueue<Runnable>();

        private Dictionary<Desire, CircumstanceListener> listenersMap;

        public Reasoner(Assets.Code.Agent.Agent agent, Circumstance c, AgentArchitecture ar)
        {
            ag = agent;
            agArch = ar;

            if (c == null)
            {
                C = new Circumstance();
            }
            if (agent != null) agent.SetReasoner(this);
            if (ar != null) ar.SetReasoner(this);
        }

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
                    if(d is DesireListenerForMetaEvents)
                    {
                        return; 
                    }
                }

                CircumstanceListener cl = new CircumstanceListener();
                C.AddEventListener(cl);
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
                C.RemoveEventListener(cl);
            }

            return desireListeners.Remove(desire);
        }



        //It's the reasoning cycle for the agent
        //Is an infinite loop
        public void ReasoningCycle()
        {
            Sense();
            Deliberate();
            Act();
        }

        public void Sense()
        {
            try
            {
                GetUserAgArch().ReasoningCycleStarting();
                C.ResetSense();

                if(nrcslbr >= DEFAULT_NUMBER_REASONING_CYCLES)
                {
                    nrcslbr = 0;
                    //Here there is something synchronized
                    /*
                        syncrhonized (C.syncApPlanSense){
                            ag.buf(GetUserAgArch().perceive());
                        }
                    */
                    GetUserAgArch().CheckMail();
                }
                nrcslbr++;

                //produce sleeps events
                if(CanSleep())
                {
                    if (!sleepingEvt)
                    {
                        sleepingEvt = true;
                        if (ag.GetPlanLibrary().GetCandidatePlans(PlanLibrary.TE_JAG_SLEEPING) != null)
                        {
                            C.AddExternalEv(PlanLibrary.TE_JAG_SLEEPING);
                        }
                    }
                } else if (sleepingEvt)
                {
                    if (C.HashMsg())
                    {
                        sleepingEvt = false;
                    } else if (C.HasEvt())
                    {
                        foreach(Event e in C.GetEvents())
                        {
                            Intention i = e.GetIntention();
                            if(!e.GetTrigger().Equals(PlanLibrary.TE_JAG_SLEEPING) || (i!=null && i.HasTrigger(PlanLibrary.TE_JAG_SLEEPING, new Unifier())){
                                sleepingEvt = false;
                                break;
                            }
                        }
                    }
                    if(!sleepingEvt && ag.GetPlanLibrary().GetCandidatePlans(PlanLibrary.TE_JAG_AWAKING) != null)
                    {
                        C.AddExternalEv(PlanLibrary.TE_JAG_AWAKING);
                    }
                }

                stepSense = State.StartRC;
                do
                {
                    ApplySemanticRuleSense();
                } while (stepSense != State.SelEv && GetUserAgArch().IsRunning());
            } catch(Exception e)
            {

            }
        }

        public void Deliberate()
        {
            try
            {
                C.ResetDeliberate();
                Thread t = taskForBeginOfCycle.Poll();
                while (t != null)
                {
                    t.Run();
                    t = taskForBeginOfCycle.Poll();
                }

                stepDeliberate = State.SelEv;
                do
                {
                    ApplySemanticRuleDeliberate();
                } while (stepDeliberate != State.ProcAct && GetUserAgArch().IsRunning());
            } catch(Exception e)
            {

            }
        }

        public void Act()
        {
            try
            {
                C.ResetAct();
                stepAct = State.ProAct;
                do
                {
                    ApplySemanticRuleAct();
                } while (stepAct != State.StartRC && GetUserAgArch().IsRunning());

                ExecuteAction action = C.GetAction();
                if(action != null)
                {
                    C.AddPendingAction(action);
                    GetUserAgArch().Act(action);
                }
            } catch (Exception e)
            {
                
            }
        }
    }
}
