using Assets.Code.Syntax;
using Assets.Code.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.BDI
{
    public class Reasoner //This might not be a runnable 
    {
        private Agent ag;

        //Constructora del razonador con el agente???
        public Reasoner(Agent agent)
        {
            ag = agent;
        }
        
        public Dictionary<string, string> Perceive()
        {
            //Makes the agent perceive the environment
            return ag.Perceive();
        }

        public void UpdateBeliefs(Dictionary<string, string> percepts)
        {
            //Makes the agent update it's belief base
            ag.UpdateBeliefBase(percepts);

        }

        public Plan SelectPlan(Desire d)
        {
            //with the selected desire checks for a plan that matches (?)
            //Retrieves the relevant plans, determines the applicable plan
            //Selects one plan. 

            return ag.GetCurrentPlan(); // ???
        }

        public void Act(Plan plan)
        {
            //Gets the plan body of the plan, and enqueues the actions in the executor
            ag.Act();
        }

        public void Run()
        {
            ag.SetReasoning(true);
            Dictionary<string, string> p = Perceive();
            UpdateBeliefs(p);
            Desire d = ag.SelectDesire();
            Plan i = SelectPlan(d);
            Act(i);
            ag.SetReasoning(false);
        }


        //This maybe needs more methods but we don't know them yet
    }   
}
