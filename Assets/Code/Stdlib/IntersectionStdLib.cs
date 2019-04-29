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
    public class IntersectionStdLib:DifferenceStdLib
    {
        // to inherit checkArgs

        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
                singleton = new IntersectionStdLib();
            return singleton;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return un.Unifies(args[2], ((IListTerm)args[0]).Intersection((IListTerm)args[1]));
        }
    }
}
