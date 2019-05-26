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

        private Intention currentPlan;
        private Desire currentDesire;

        public Agent(string name, string asl)
        {
            beliefBase = new List<Belief>();
            planLibrary = new List<Intention>();
            desires = new List<Desire>();
            agentName = name;
            aslSourcePath = asl;
            reasoner = new Reasoner(this);  // We think Reasoner needs an Agent, maybe?
        }

        //Getters and setters for the list
        public List<Belief> GetBeliefBase() => beliefBase;
        public void SetBeliefBase(List<Belief> bb) => beliefBase = bb;

        public List<Intention> GetPlanLibrary() => planLibrary;
        public void SetPlanLibrary(List<Intention> pl) => planLibrary = pl;

        public List<Desire> GetDesires() => desires;
        public void SetDesires(List<Desire> ds) => desires = ds;

        public Intention GetCurrentPlan => currentPlan;
        public Desire GetCurrentDesire => currentDesire;

        public void Act()
        {
            //Here the agent acts in the environment. Since this has a lot to do with unity, 
            //Irene will do it
        }

        // Gets the first plan in the list
        public void SelectPlan()
        {
            currentPlan = planLibrary[0];
            // Should this remove the plan from the list afterwards?
        }

        // Gets the first desire in the list
        public void SelectDesire()
        {
            currentDesire = desires[0];
            // Same, should this remove the desire afterwards?
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

        // Calls the parser and retrieves the lists
        public void Parse()
        {
            // Call parser somehow

            SetBeliefBase(/*parser*/);
            SetPlanLibrary(/*parser*/);
            SetDesires(/*parser*/);
        }

        //There are more methods here but we don't know them yet
    }
}