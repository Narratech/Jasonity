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
    public class Df_Subscribe: Df_Register
    {
        private static IInternalAction singleton = null;
        public static IInternalAction Create()
        {
            if (singleton == null)
                singleton = new Df_Subscribe();
            return singleton;
        }

        public int GetMinArgs()
        {
            return 1;
        }

        public int GetMaxArgs()
        {
            return 2;
        }

        public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ts.GetUserAgArch().GetRuntimeServices().DfSubscribe(ts.GetUserAgArch().GetName(), GetService(args), GetType(args));
            return true;
        }
    }
}
