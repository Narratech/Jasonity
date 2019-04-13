using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: removes events <i>D</i> from the agent circumstance.
 * This internal action simply removes all <i>+!D</i> entries
 */
namespace Assets.Code.Stdlib
{
    public class Drop_Event:Drop_Desire
    {
        public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            DropEvt(ts.GetCircumstance(), args[0] as Literal, un);
            return true;
        }
    }
}
