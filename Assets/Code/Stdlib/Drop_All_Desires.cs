using Assets.Code.Logic.AsSyntax;
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
    public class Drop_All_Desires:Drop_All_Intentions
    {
        public object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            base.Execute(ts, un, args);
            ts.GetC().ClearEvents();
            ts.GetC().ClearPendingEvents();
            return true;
        }
    }
}
