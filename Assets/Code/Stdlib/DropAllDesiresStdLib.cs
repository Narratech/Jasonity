using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: removes all desires of the agent. No event is
 * produced.
 */
namespace Assets.Code.Stdlib
{
    public class DropAllDesiresStdLib:DropAllIntentionsStdLib
    {
        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            base.Execute(ts, un, args);
            ts.GetCircumstance().ClearEvents();
            ts.GetCircumstance().ClearPendingEvents();
            return true;
        }
    }
}
