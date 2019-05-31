using Assets.Code.Actions;
using Assets.Code.Syntax;
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

        public string GetName()
        {
            return agentName;
        }

        public void SetPlanLibrary(List<Plan> pl) => planLibrary = pl;

        public List<Desire> GetDesires() => desires;
        public void SetDesires(List<Desire> ds) => desires = ds;

        public Plan GetCurrentPlan() => currentPlan;
        public Desire GetCurrentDesire() => currentDesire;

        public void Act()
        {
            Debug.Log(ToString() + ": Estoy actuando");
            Plan p = planLibrary[0];

            List<Syntax.Action> actions = p.PlanBody.Actions;
            foreach (Syntax.Action a in actions)
            {
                string function = a.ActionName;
                string[] arguments = a.Arguments;
                switch (function)
                {
                    case "TurnOnLight":
                        TurnOnLight t = new TurnOnLight();
                        GameObject g = GameObject.Find(arguments[0]);
                        Light l = g.GetComponent<Light>();
                        l.intensity = 1;
                        t.Run(g);
                        break;
                    case "TurnOffLight":
                        TurnOffLight tl = new TurnOffLight();
                        GameObject go = GameObject.Find(arguments[0]);
                        
                        tl.Run(go);
                        break;
                    default:
                        break;
                }
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
        
        public Dictionary<string, string> Perceive()
        {
            Dictionary<string, string> percepts = new Dictionary<string, string>();
            Debug.Log(ToString() + ": Estoy percibiendo el entorno");

            foreach (GameObject g in GameController.environment)
            {
                string s = g.GetComponent<IEnvironmentObject>().GetPercepts();
                string[] aux = s.Split(':');
                percepts.Add(aux[0], aux[1]);
            }

            return percepts;
        }

        public void UpdateBeliefBase(Dictionary<string, string> percepts)
        {
            //With the perceptions checks the belief base and deletes the beliefs that are
            //no longer correct and adds the new ones
            Debug.Log(ToString() + ": Estoy actualizando la base de creencias");
            foreach (string obj in percepts.Keys)//check if all the new perceptions are in the belief base and add them if theyre not
            {
                bool encontrado = false;
                foreach (Belief bel in beliefBase)
                {
                    string[] aux = bel.GetBelief().Split(':');
                    if (obj.Equals(aux[0])){
                        bel.UpdateValue(aux[1]);
                        encontrado = true;
                    }
                }
                if(!encontrado)
                {
                    string s;
                    percepts.TryGetValue(obj, out s);
                    beliefBase.Add(new Belief(obj, s));
                }
            }

            foreach (Belief bel in beliefBase)//Checks the belief base in case there are beliefs no longer perceived
            {
                if(!percepts.ContainsKey(bel.GetName()))
                {
                    beliefBase.Remove(bel);
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
        

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(agentName))
                agentName = "Agent";
            return agentName;
        }

        public Reasoner GetReasoner()
        {
            return reasoner;
        }
    }
}