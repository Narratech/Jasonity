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
    public class UnionStdLib:DifferenceStdLib //to inherit checkArguments
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new UnionStdLib();
            return singleton;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return un.Unifies(args[2], ((IListTerm) args[0]).Union((IListTerm) args[1]) );
        }
    }
}
