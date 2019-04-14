using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.Agent;
using Assets.Code.Mas2J;

namespace Assets.Code.Runtime
{
    public interface RuntimeServices
    {
        IEnumerable<string> GetAgentsNames();
        void Clone(Agent.Agent agent, object p, string agName);
        string CreateAgent(string name, string source, string agClass, List<string> agArchClasses, ClassParameters bbPars, Code.Settings settings, Agent.Agent agent);
        void StartAgent(string name);
        void DfDeRegister(string v1, string v2, string v3);
        void DfRegister(string v1, string v2, string v3);
        IEnumerable<string> DfSearch(string v1, string v2);
        void DfSubscribe(string v1, string v2, string v3);
    }
}
