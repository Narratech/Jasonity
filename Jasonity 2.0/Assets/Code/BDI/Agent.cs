using Assets.Code.Syntax;
using System.Collections.Generic;

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

        public Agent(string name, string asl)
        {
            beliefBase = new List<Belief>();
            planLibrary = new List<Plan>();
            desires = new List<Desire>();
            agentName = name;
            aslSourcePath = asl;
            reasoner = new Reasoner(this);
            Parse();
        }

        //Getters and setters for the list
        public List<Belief> GetBeliefBase() => beliefBase;
        public void SetBeliefBase(List<Belief> bb) => beliefBase = bb;

        public List<Plan> GetPlanLibrary() => planLibrary;
        public void SetPlanLibrary(List<Plan> pl) => planLibrary = pl;

        public List<Desire> GetDesires() => desires;
        public void SetDesires(List<Desire> ds) => desires = ds;

        public Plan GetCurrentPlan() => currentPlan;
        public Desire GetCurrentDesire() => currentDesire;

        public void Act()
        {
            //Here the agent acts in the environment. Since this has a lot to do with unity, 
            //Irene will do it
        }

        // Gets the first plan in the list
        public Plan SelectPlan()
        {
            return currentPlan = planLibrary[0];
        }

        // Gets the first desire in the list
        public Desire SelectDesire()
        {
            return currentDesire = desires[0];
            // Same, should this remove the desire afterwards?
        }

        public void RemovePlan(Plan plan) { }

        public void RemoveDesire(Desire desire) { }

        public void RemoveBelief(Belief belief) { }

        public void Perceive(Dictionary<string, string> percept)
        {
            //This has a lot to do with unity. Ask Irene.
        }

        public void UpdateBeliefBase()
        {
            //With the perceptions checks the belief base and deletes the beliefs that are
            //no longer correct and adds the new ones

        }

        // Calls the parser and retrieves the lists
        public void Parse()
        {
            // Call parser somehow
            ParserClass parser = new ParserClass();
            parser.Parser(aslSourcePath);
            beliefBase = parser.GetBeliefs();
            planLibrary = parser.GetPlans();
            desires = parser.GetDesires();
        }

        //There are more methods here but we don't know them yet
    }
}