using Assets.Code.Syntax;
using Assets.Code.Utilities;

namespace Assets.Code.BDI
{
    public class Reasoner:IRunnable //This might not be a runnable 
    {
        private Agent ag;

        //Constructora del razonador con el agente???
        public Reasoner(Agent agent)
        {
            ag = agent;
        }
        
        public void Perceive()
        {
            //Makes the agent perceive the environment
        }

        public void UpdateBeliefs()
        {
            //Makes the agent update it's belief base
            ag.UpdateBeliefBase();

        }

        public Plan SelectPlan()
        {
            //Retrieves the relevant plans, determines the applicable plan
            //Selects one plan. 

            return ag.GetCurrentPlan(); // ???
        }

        public void Act(Plan plan)
        {
            //Gets the plan body of the plan, and enqueues the actions in the executor
        }

        public void Run()
        {
            Perceive();
            UpdateBeliefs();
            Plan i = SelectPlan();
            Act(i);
        }

        //This maybe needs more methods but we don't know them yet
    }   
}
