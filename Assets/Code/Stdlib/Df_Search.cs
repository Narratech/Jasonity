using Assets.Code.Logic;
using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Df_Search:Df_Register
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new Df_Search();
            }
            return singleton;
        }

        public int GetMinArgs()
        {
            return 2;
        }

        public int GetMaxArgs()
        {
            return 3;
        }

        public object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            ListTerm lt = new ListTermImpl();
            foreach (string a in ts.GetUserAgArch().GetRuntimeServices().DfSearch(GetService(args), GetType(args)))
            {
                lt.Add(new Atom(a));
            }
            return un.Unifies(args[args.Length-1], lt);
        }
    }
}
