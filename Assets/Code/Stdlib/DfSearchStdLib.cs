using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class DfSearchStdLib:DfRegisterStdLib
    {
        private static InternalAction singleton = null;
        public new static InternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new DfSearchStdLib();
            }
            return singleton;
        }

        public override int GetMinArgs()
        {
            return 2;
        }

        public override int GetMaxArgs()
        {
            return 3;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            IListTerm lt = new ListTermImpl();
            foreach (string a in ts.GetUserAgArch().GetRuntimeServices().DfSearch(GetService(args), GetType(args)))
            {
                lt.Add(new Atom(a));
            }
            return un.Unifies(args[args.Length-1], lt);
        }
    }
}
