using Assets.Code.Logic;
using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Df_Deregister: Df_Register
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
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

        public object Excute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            ts.GetUserAgArch().GetRuntimeServices().DfDeRegister(ts.GetUserAgArch().GetAgName(), GetService(args), GetType(args));
            return true;
        }
    }
}
