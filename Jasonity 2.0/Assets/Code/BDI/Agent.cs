using Assets.Code.Actions;
using Assets.Code.Syntax;
using Assets.Code.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.BDI
{
    public class Agent
    {
        private List<Belief> beliefBase;
        private List<Plan> planLibrary;
        private List<Desire> desires;
        private string agentName;
        private string aslSourcePath;
        private Reasoner reasoner;

        private Plan currentPlan;
        private Desire currentDesire;
        private bool isReasoning;

        public Agent(string name, string asl)
        {
            beliefBase = new List<Belief>();
            planLibrary = new List<Plan>();
            desires = new List<Desire>();
            agentName = name;
            aslSourcePath = asl;
            reasoner = new Reasoner(this);
            isReasoning = false;
            Parse();
        }

        public bool IsReasoning()
        {
            return isReasoning;
        }

        //Getters and setters for the list
        public List<Belief> GetBeliefBase() => beliefBase;
        public void SetBeliefBase(List<Belief> bb) => beliefBase = bb;

        public List<Plan> GetPlanLibrary() => planLibrary;

        public void SetReasoning(bool reasoning)
        {
            isReasoning = reasoning;
        }

        public void SetPlanLibrary(List<Plan> pl) => planLibrary = pl;

        public List<Desire> GetDesires() => desires;
        public void SetDesires(List<Desire> ds) => desires = ds;

        public Plan GetCurrentPlan() => currentPlan;
        public Desire GetCurrentDesire() => currentDesire;

        public void Act()
        {
            //Here the agent acts in the environment. Since this has a lot to do with unity, 
            //Irene will do it
            Debug.Log(ToString() + ": Estoy actuando");
            Plan p = planLibrary[0];
            
            //TO TEST THE ACTIONS
            if(agentName.Equals("light_on_agent"))
            {
                TurnOnLight act = new TurnOnLight(GameObject.Find("LampPlaceholder"));
                act.Run();
            } else
            {
                TurnOffLight act = new TurnOffLight(GameObject.Find("LampPlaceholder"));
                act.Run();
            }
        }

        // Gets the first plan in the list
        public Plan SelectPlan(Desire d)
        {
            //Para el deseo d
            Debug.Log(ToString() + ": Estoy seleccionando un plan");
            return currentPlan = planLibrary[0];
        }

        // Gets the first desire in the list
        public Desire SelectDesire()
        {
            Debug.Log(ToString() + ": Estoy seleccionando un deseo");
            //return currentDesire = desires[0];
            // Same, should this remove the desire afterwards
            return null;
        }

        public void RemovePlan(Plan plan) { }

        public void RemoveDesire(Desire desire) { }

        public void RemoveBelief(Belief belief) { }

        public Dictionary<string, string> Perceive()
        {
            Dictionary<string, string> percepts = new Dictionary<string, string>();
            Debug.Log(ToString() + ": Estoy percibiendo el entorno");

            //foreach(GameObject g in gc.environment)
            //{
            //    string s = g.GetComponent<IEnvironmentObject>().GetPercepts();
            //    string[] aux = s.Split(':');
            //    percepts.Add(aux[0], aux[1]);
            //}

            return percepts;
        }

        public void UpdateBeliefBase(Dictionary<string, string> percepts)
        {
            //With the perceptions checks the belief base and deletes the beliefs that are
            //no longer correct and adds the new ones
            Debug.Log(ToString() + ": Estoy actualizando la base de creencias");
            foreach (string obj in percepts.Keys)
            {
                foreach (Belief bel in beliefBase)
                {
                    string[] aux = bel.GetBelief().Split(':');
                    if (obj.Equals(aux[0])){

                    }
                    Debug.Log("Actualizo creencia");
                }
            }
        }

        // Calls the parser and retrieves the lists
        public void Parse()
        {
            // Call parser somehow
            Parser parser = new Parser();
            parser.Parse(aslSourcePath);
            beliefBase = parser.GetBeliefs();
            planLibrary = parser.GetPlans();
            desires = parser.GetDesires();
        }

        //There are more methods here but we don't know them yet

        public void Run()
        {
            reasoner.Run();
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(agentName))
                agentName = "Agent";
            return agentName;
        }
    }
}