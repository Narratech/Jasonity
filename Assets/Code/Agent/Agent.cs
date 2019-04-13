using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.AsSyntax;
using Assets.Code.Logic;
using Assets.Code.Mas2J;
using Assets.Code.ReasoningCycle;
using BDIManager.Beliefs;
using BDIManager.Intentions;
/**
 * The agent class has the belief base and the library plan
 */
namespace Assets.Code.Agent
{
    public class Agent
    {
        private IBeliefBase bb = null;
        private PlanLibrary pl = null;
        private Reasoner reasoner = null;
        private string aslSource = null;
        //The ones in the source code
        private List<Literal> initialGoals = null;
        private List<Literal> initialBeliefs = null;
        private Dictionary<string, IInternalAction> internalActions = null;
        private Dictionary<string, ArithFunctionTerm> functions = null;
        private bool hasCustomSelOp = true;
        //private static ScheduledExecutorService scheduler = null; //I don't know how to do this

        public Agent()
        {
            CheckCustomSelectOption();
        }

        public static Agent Create(AgentArchitecture agArch, string agClass, ClassParameters bbPars, string asSrc, Settings stts)  
        {
            try
            {
                //Agent ag = (Agent) Class.forName(agClass).newInstance(); //???
                Agent ag = new Agent();
                Reasoner r = new Reasoner(ag, null, agArch, stts);
                IBeliefBase bb = null;
                if (bbPars == null)
                {
                    bb = new DefaultBeliefBase();
                } else
                {
                    //bb = (BeliefBase) Class.forName(bbPars.getClassName()).newInstance();
                }

                ag.SetBB(bb);
                ag.InitAg();

                if (bbPars != null)
                {
                    bb.Init(ag, bbPars.GetParametersArray());
                }

                ag.Load(asSrc);
                return ag;
            }
            catch (Exception e)
            {
                throw new JasonException(e);
            }
        }

        public void InitAg()
        {
            if (bb == null)
            {
                new DefaultBeliefBase();
            }

            if (pl == null)
            {
                pl = new PlanLibrary();
            }

            if (initialGoals == null)
            {
                initialGoals = new List<Literal>();
            }

            if (initialBeliefs == null)
            {
                initialBeliefs = new List<Literal>();
            }

            if (internalActions == null)
            {
                internalActions = new Dictionary<string, InternalAction>();
            }

            //if (! "false".equals(Config.get().getProperty(Config.START_WEB_MI))) MindInspectorWeb.get().registerAg(this);
        }

        public void InitAg(string asSrc)
        {
            InitAg();
            Load(asSrc);
        }

        public void Load(string asSrc)
        {
            try
            {
                bool parsingOk = true;
                if(asSrc != null && !string.IsNullOrEmpty(asSrc))
                {
                    asSrc = asSrc.Replace("\\\\", "/");

                    if (asSrc.StartsWith(SourcePath.CRPrefix)) //I don't know yet what the hell is SourcePath.CRPrefix
                    {
                        //parseAS(Agent.class.getResource(asSrc.substring(SourcePath.CRPrefix.length())).openStream() , asSrc); I don't know what this is
                    } else
                    {
                        try
                        {
                            parsingOk = ParseAs(new URL(asSrc)); //I need to find a replace for url but i don't understand why this is used
                        } catch (Exception e)
                        {

                        }
                     }

                }
            } catch (Exception e)
            {

            }
        }

        public string GetASLSrc()
        {
            return aslSource;
        }

        public void AddInitialBel(Literal b)
        {
            initialBeliefs.Add(b);
        }

        public void AddInitialGoal(Literal g)
        {
            initialGoals.Add(g);
        }

        public PlanLibrary GetPL()
        {
            return pl;
        }
    }
}

