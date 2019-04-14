using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: subscribes the agent as interested in providers of service S of type T.
 * For each new agent providing this service, the agent will receive a message
 */
namespace Assets.Code.Stdlib
{
    public class DfSubscribeStdLib: DfRegisterStdLib
    {
        private static InternalAction singleton = null;
        public new static InternalAction Create()
        {
            if (singleton == null)
                singleton = new DfSubscribeStdLib();
            return singleton;
        }

        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ts.GetUserAgArch().GetRuntimeServices().DfSubscribe(ts.GetUserAgArch().GetAgentName(), GetService(args), GetType(args));
            return true;
        }
    }
}
