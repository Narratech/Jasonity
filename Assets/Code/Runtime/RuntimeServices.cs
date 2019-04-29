using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.BDIAgent;
using Assets.Code.Mas2J;

namespace Assets.Code.Runtime
{
    public interface RuntimeServices
    {
        /**
         * Creates a new agent with <i>agName</i> from source
         * <i>agSource</i>, using <i>agClass</i> as agent class (default
         * value is "jason.asSemantics.Agent"), <i>archClasses</i> as agent
         * architecture classes,
         * <i>bbPars</i> as the belief base
         * class (default value is "DefaultBeliefBase"), <i>stts</i> as
         * Settings (default value is new Settings()), and
         * <i>father</i> is the agent creating this agent (null is none).
         * 
         * if no archClasses is informed (null value), 
         *    if fathers is informed
         *        use father's ag archs
         *    else
         *        use default ag archs (see registerDefaultAgArch) 
         *
         * <p> Example: createAgent("bob", "bob.asl", "mypkg.MyAgent", null, null, null);
         *
         * Returns the name of the agent
         */
        string CreateAgent(string name, string source, string agClass, List<string> agArchClasses,
            ClassParameters bbPars, Code.Settings settings, Agent agent);

        string GetNewAgentName(string name);

        void Clone(Agent agent, object p, string agName);

        /** register a class to be included as new agents archs */
        void RegisterDefaultAgArch(string agArch);

        IEnumerable<string> GetDefaultAgArchs();

        /** starts an agent (e.g. create thread for it) */
        void StartAgent(string name);

        /**
         * Clones an agent
         *
         * @param source: the agent used as source for beliefs, plans, ...
         * @param archClassName: agent architectures that will be used
         * @param agName: the name of the clone
         * @return the agent arch created
         * @throws JasonException
         */
        AgentArchitecture Clone(Agent source, IEnumerable<string> archClasses, string agName);

        /**
        * Kills the agent named <i>agName</i> as a requested by <i>byAg</i>.
        * Agent.stopAg() method is called before the agent is removed.
        */
        object KillAgent(string name, string v);

        /** Returns a set of all agents' name */
        IEnumerable<string> GetAgentsNames();

        /** Gets the number of agents in the MAS. */
        int GetAgentsQty();

        /** Stops all MAS (the agents, the environment, the controller, ...) */
        void StopMAS();

        void DfDeRegister(string v1, string v2, string v3);

        void DfRegister(string v1, string v2, string v3);

        IEnumerable<string> DfSearch(string v1, string v2);

        void DfSubscribe(string v1, string v2, string v3);
    }
}
