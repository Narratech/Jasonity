using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using BDIManager.Beliefs;
using BDIManager.Intentions;

/**
 * The agent class has the belief base and the library plan
 */
namespace Assets.Code.Agent
{
    class Agent
    {
        protected BeliefBase bb = null;
        protected PlanLibrary planLibrary = null;
        protected String aslSource = null;
        protected Reasoner reasoner = null; //Reference to the reasoner

        private List<Literal> initialGoals = null;
        private List<Literal> initialBeliefs = null;

        //This might be needed in the future
        //private Dictionary<String, InternalAction> internalActions = null;
        //private Dictionary<String, ArithFunction> functions = null;

        public Agent()
        {

        }

        /**
         * Creates an agent and it's belief base
         */
        public static Agent Create(AgentArchitecture agArch, String agClass, String asSrc)
        {
            try
            {
                Agent ag = new Agent();
                ag.bb = new DefaultBeliefBase();
                ag.InitAgente();
                ag.Load(asSrc);
                Reasoner r = new Reasoner(ag, null, agArch);
                ag.reasoner = r;
                return ag;
            }
            catch (Exception e)
            {
                throw new Exception("Error creating the agent: " + e.Message);
            }
        }

        public void SetReasoner(Reasoner r)
        {
            reasoner = r;
        }

        /**
         * Initializes the belief base, the plan library, the initial goals and the initial beliefs of the agent 
         */
        public void InitAgente()
        {
            if (bb == null)
                bb = new DefaultBeliefBase();
            if (planLibrary == null)
                planLibrary = new PlanLibrary();
            if (initialGoals == null)
                initialGoals = new List<Literal>();
            if (initialBeliefs == null)
                initialBeliefs = new List<Literal>();
        }

        /**
         *  Parse and load the source code of the agent
         */
        public void Load(String asSrc)
        {
            try
            {
                if (!String.IsNullOrEmpty(asSrc)) //If the source isn't null or empty
                {
                    asSrc = asSrc.Replace("\\", "/");

                    AddInitialBeliefsFromProjectInBB();
                    AddInitialBeliefsInBB();
                    AddInitialGoalsFromProjetInBB();


                    aslSource = asSrc;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading the file: " + e.Message);
            }
        }

        public void StopAgent()
        {
            //This maybe needs to be synchronized
            bb.Stop();
        }

        //This is not implemented yet but might be needed in the future
        public void Clone()
        {
            throw new NotImplementedException();
        }

    }
}

