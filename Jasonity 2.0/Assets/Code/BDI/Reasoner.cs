using Assets.Code.Syntax;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.BDI
{
    public class Reasoner
    {
        private Agent ag;

        public Reasoner(Agent agent)
        {
            ag = agent;
        }
        
        public Dictionary<string, string> Perceive()
        {
            return ag.Perceive();
        }

        public void UpdateBeliefs(Dictionary<string, string> percepts)
        {
            ag.UpdateBeliefBase(percepts);
        }

        public Plan SelectPlan()
        {
            return ag.GetCurrentPlan();
        }

        public void Act(Plan plan)
        {
            ag.Act();
        }
    }   
}
