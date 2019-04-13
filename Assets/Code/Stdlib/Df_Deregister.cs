using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Df_Deregister: Df_Register
    {
        private static IInternalAction singleton = null;
        public static IInternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new Df_Deregister();
            }
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

        public object Excute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ts.GetUserAgArch().GetRuntimeServices().DfDeRegister(ts.GetUserAgArch().GetAgentName(), GetService(args), GetType(args));
            return true;
        }
    }
}
