using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using UnityEngine;
using Assets.Code.ReasoningCycle;
using BDIMaAssets.Code.ReasoningCycle;

/*
    THis is the base architecture for agents.
    Implements a chain of responsability design pattern. The chain is made of
    AgentArchitecture and the last one is an AgArchitecture. 
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
        
        public void SetReasoner(Reasoner r)
        {
            reasoner = r;
        }

        public void ReasoningCycleStarting()
        {
            if(this.successor != null)
            {
                successor.ReasoningCycleStarting();
            }
        }

        public List<Literal> Perceive()
        {
            if (successor == null)
            {
                return null;
            } else
            {
                return successor.Perceive();
            }   
        }

        public void CheckMail()
        {
            if(successor != null)
            {
                successor.CheckMail();
            }
        }

        public void Act(ExecuteAction action)
        {
            throw new NotImplementedException();
        }

        internal bool IsRunning()
        {
            throw new NotImplementedException();
        }

        internal string GetAgName()
        {
            throw new NotImplementedException();
        }

        internal bool CanSleep()
        {
            throw new NotImplementedException();
        }
    }
}
