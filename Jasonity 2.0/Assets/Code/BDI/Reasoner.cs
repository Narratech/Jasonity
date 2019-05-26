using Assets.Code.Utilities;

namespace Assets.Code.BDI
{
    public class Reasoner:IRunnable //This might not be a runnable 
    {
        public Reasoner()
        {

        }
        
        public void Perceive()
        {
            //Makes the agent perceive the environment
        }

        public void UpdateBeliefs()
        {
            //Makes the agent update it's belief base
        }

        public Intention SelectPlan()
        {
            //Retrieves the relevant plans, determines the applicable plan
            //Selects one plan. 
            return null;
        }

        public void Act(Intention plan)
        {
            //Gets the plan body of the plan, and enqueues the actions in the executor
        }

        public void Run()
        {
            Perceive();
            UpdateBeliefs();
            Intention i = SelectPlan();
            Act(i);
        }

        //This maybe needs more methods but we don't know them yet
    }   
}
