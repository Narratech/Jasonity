using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using UnityEngine;
using Assets.Code.ReasoningCycle;
using BDIMaAssets.Code.ReasoningCycle;

/*
    This is the agent architecture class, defines the agent architecture. 
    Implements a chain of responsability pattern where each member of the chain is a subclass and
    the last arch in the chain is a specific architecture.
    The reasoner calls the methods to get perception, action and communication.
*/


namespace Assets.Code.Agent
{
    class AgentArchitecture
    {
        private Reasoner reasoner = null;

        //Chain of responsability
        private AgentArchitecture successor = null;
        private AgentArchitecture firstArch = null;

        //current cycle number
        private int cycleNumber = 0;

        public AgentArchitecture()
        {
            firstArch = this;
        }

        public void Stop()
        {
            if (successor != null)
            {
                successor.Stop();
            }
        }

        public AgentArchitecture GetFirstAgentArchitecture()
        {
            return firstArch;
        }

        public AgentArchitecture GetNextAgentArchitecture()
        {
            return successor;
        }

        public List<string> GetAgentArchitectureClassesChain()
        {
            List<string> all = new List<string>();
            AgentArchitecture a = GetFirstAgentArchitecture();
            while (a != null)
            {
                all.Insert(0, a.GetType().ToString());
                a = a.GetNextAgentArchitecture();
            }

            return all;
        }

        public void InsertAgentArchitecture(AgentArchitecture agArch)
        {
            if (agArch != GetFirstAgentArchitecture())
            {
                agArch.successor = GetFirstAgentArchitecture();
            }
            if (reasoner != null)
            {
                agArch.SetReasoner(this.GetReasoner());
                GetReasoner().SetAgArch(agArch);
            }
            SetFirstAgArch(agArch);
        }

        private void SetFirstAgArch(AgentArchitecture agArch)
        {
            firstArch = agArch;
            if (successor != null)
            {
                successor.SetFirstAgArch(agArch);
            }
        }
        
        //I think we won't need this because we won't allow custom stuff but i'm putting here the method just in case
        public void CreateCustomArchs (List<string> archs)
        {
            throw new NotImplementedException("CreateCustomArchs not implemented");
        }

        public void ReasoningCycleStarting()
        {
            if (successor != null)
            {
                successor.ReasoningCycleStarting();
            }
        }

        public void SetReasoner(Reasoner reasoner)
        {
            this.reasoner = reasoner;
            if (successor != null)
            {
                successor.SetReasoner(reasoner);
            }
        }

        public Reasoner GetReasoner()
        {
            if (reasoner != null)
            {
                return reasoner;
            } 
            if (successor != null)
            {
                return successor.GetReasoner();
            }
            return null;
        }

        //Gets the agent's perception as a list of Literals
        public List<Literal> perceive()
        {
            if (successor == null) {
                return null;
            } else
            {
                return successor.perceive();
            }
        }

        //Check if the agent has any messages
        public void CheckMail()
        {
            if (successor != null)
            {
                successor.CheckMail();
            }
        }

        //Executes the action
        public void Act(ExecuteAction action)
        {
            if (successor != null)
            {
                successor.Act(action);
            }
        }

        //Inform when the action execution is finished
        public void ActionExecuted(ExecuteAction action)
        {
            GetReasoner().GetCircumstance().AddFeedbackAction(action);
            WakeUpAct();
        }

        public bool CanSleep()
        {
            return (successor == null) || successor.CanSleep();
        }

        public void Wake()
        {
            if (successor != null)
            {
                successor.Wake();
            }
        }

        public void WakeUpSense()
        {
            if (successor != null)
            {
                successor.WakeUpSense();
            }
        }

        public void WakeUpDeliberate()
        {
            if(successor != null)
            {
                successor.WakeUpDeliberate();
            }
        }

        public void WakeUpAct()
        {
            if (successor != null)
            {
                successor.WakeUpAct();
            }
        }

        public RuntimeServices GetRuntimeServices()
        {
            if (successor == null)
            {
                return null;
            } else
            {
                return successor.GetRuntimeServices();
            }
        }

        public string GetAgentName()
        {
            if (successor == null)
            {
                return "no-named";
            } else
            {
                return successor.GetAgentName();
            }
        }

        public void SendMessage(Message m )
        {
            if (successor != null)
            {
                successor.SendMessage(m);
            }
        }

        public void Broadcast(Message m)
        {
            if (successor != null)
            {
                successor.Broadcast(m);
            }
        }

        public bool IsRunning()
        {
            return successor == null || successor.IsRunning();
        }

        public void SetCycleNumber(int c)
        {
            cycleNumber = c;
            if (successor != null)
            {
                successor.SetCycleNumber(c);
            }
        }

        public int GetCycleNumber()
        {
            return cycleNumber;
        }
    }
}
