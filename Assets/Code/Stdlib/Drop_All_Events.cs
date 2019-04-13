using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Drop_All_Events:DefaultInternalAction
    {
        public int GetMinArgs()
        {
            return 0;
        }
        public int GetMaxArgs()
        {
            return 0;
        }

        public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ts.GetCircumstance().ClearEvents();
            ts.GetCircumstance().ClearPendingEvents();
            return true;
        }
    }
}
