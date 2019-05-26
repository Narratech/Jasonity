using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Code.BDI
{
    public class Agent
    {
        private List<Belief> beliefBase;
        private List<Intention> planLibrary;
        private List<Desire> desires;
        private string agentName;
        private string aslSourcePath;
        private Reasoner reasoner;

        public Agent(string name, string asl)
        {
            beliefBase = new List<Belief>();
            planLibrary = new List<Intention>();
            desires = new List<Desire>();
            agentName = name;
            aslSourcePath = asl;
            reasoner = new Reasoner();
        }
        
        //Getters and setters for the list

        public void Act()
        {
            //Here the agent acts in the environment. Since this has a lot to do with unity, 
            //Irene will do it
        }

        public void SelectPlan()
        {
            //This gets the first plan in the list
        }

        public void SelectDesire()
        {
            //This gets the first desire in the list
        }

        public void Perceive(Dictionary<string, string> percept)
        {
            //This has a lot to do with unity. Ask Irene.
        }

        public void UpdateBeliefBase()
        {
            //With the perceptions checks the belief base and deletes the beliefs that are
            //no longer correct and adds the new ones
        }

        public void Parse()
        {
            //This calls the parser and retrieves the lists
        }

        //There are more methods here but we don't know them yet
    }
}