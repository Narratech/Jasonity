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
    public class Drop_All_Events:InternalAction
    {
        public override int GetMinArgs()
        {
            return 0;
        }
        public override int GetMaxArgs()
        {
            return 0;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ts.GetCircumstance().ClearEvents();
            ts.GetCircumstance().ClearPendingEvents();
            return true;
        }
    }
}
