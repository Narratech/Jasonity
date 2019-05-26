using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using Assets.Code.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
  <p>Internal action: <b><code>.stopMAS</code></b>.
  <p>Description: aborts the execution of all agents in the multi-agent system
  (and any simulated environment too).
  <p>Example:<ul>
  <li> <code>.stopMAS</code>.</li>
  </ul>
 */

namespace Assets.Code.Stdlib
{
    public class StopMASStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 0;
        }
        override public int GetMaxArgs()
        {
            return 0;
        }

        override public bool CanBeUsedInContext()
        {
            return false;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            IRuntimeServices rs = ts.GetUserAgArch().GetRuntimeServices();
            rs.StopMAS();
            return true;
        }
    }
}
