using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDIManager.Beliefs;
using BDIManager.Intentions;
using Jason.Logic.AsSyntax;
using Logica.ASSemantic;

/**
 * La clase agente tiene la base de creencias y la librería de 
 * planes. 
 */
namespace Assets.Code.Agent
{
    class Agent
    {
        protected BeliefBase bb = null;
        protected PlanLibrary planLibrary = null;
        protected String aslSource = null;
        protected TransitionSystem ts = null;

        private List<Literal> initialGoals = null;
        private List<Literal> initialBeliefs = null;

        private Dictionary<String, InternalAction> internalActions = null;
        private Dictionary<String, ArithFunction> functions = null;

        public Agent()
        {

        }

        /**
         *  Crea un agente y su BB
         */
        public static Agent Create(AgentArchitecture agArch, String agClass, String asSrc)
        {
            try
            {
                Agent ag = new Agent();
                ag.ts = new TransitionSystem(ag, null, agArch);
                ag.bb = new DefaultBeliefBase();
                ag.initAgente();
                ag.load(asSrc);

                return ag;
            }
            catch (Exception e)
            {
                throw new Exception("Error creating the agent: " + e.Message);
            }
        }

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

            InitDefaultFunctions();

        }

        public void InitAgente(String asSrc)
        {
            try
            {
                InitAgente();
                Load(asSrc);
            }
            catch (Exception e)
            {
                throw new Exception("Error loading the source: " + e.Message);
            }
        }

        public void Load(String asSrc)
        {
            try
            {
                if (asSrc != null && (asSrc.Length > 0))
                {
                    asSrc = asSrc.Replace("\\", "/");
                    //Aqui utilizan una clase que se llama SourcePath que no he puesto porque no se si hace falta
                    if (planLibrary.hasMetaEventPlans()) //Esto no se muy bien qué es
                    {
                        ts.addGoalListener(new GoalListenerForMetaEvents(ts));
                        AddInitialBelsFromProjectInBB();
                        AddInitialBelsInBB();
                        AddInitialGoalsFromProjectInBB();
                        AddInitialGoalsInTS();
                        FixAgInIAandFunctions(this);
                    }

                    LoadKqmlPlans();
                    AddInitialBelsInBB();

                    aslSource = asSrc;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading the file: " + e.Message);
            }
        }

        public void LoadKqmlPlans()
        {
            //Esto no lo he implementado porque no se muy bien que hace
        }

        public void StopAgent()
        {
            if (bb.getLock())
            {
                bb.stop();
            }

            foreach (InternalAction ia in internalActions.Values)
            {
                try
                {
                    ia.destroy();
                }
                catch (Exception e)
                {
                    //Aqui hace un printStackTrace que no se como se hace
                }
            }
        }


        public Agent Clone(AgentArchitecture arch)
        {
            //Esto no está implementado porque no se si hace falta
            return null;
        }

        
        private void FixAgInIAandFunctions(Agent a)
        {
            //No está implementado porque no se si hace falta
        }

        public void initDefaultFunctions()
        {
            if (functions == null)
            {
                functions = new Dictionary<string, ArithFunction>(); 
            }
            addFunction();
        }

        private void addFunction(ArithFunction c)
        {

        }
    }
}
