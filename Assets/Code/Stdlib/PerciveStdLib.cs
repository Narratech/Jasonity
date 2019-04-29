using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
  <p>Internal action: <b><code>.perceive</code></b>.
  <p>Description: forces the agent architecture to do perception of the
  environment immediately. It is normally used when the number of reasoning
  cycles before perception takes place was changed (this is normally at every
  cycle).
  <p>Example:<ul>
  <li> <code>.perceive</code>.</li>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class PerciveStdLib: InternalAction
    {
        override public int GetMinArgs()
        {
            return 0;
        }
        override public int GetMaxArgs()
        {
            return 0;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ts.GetAgent().Buf(ts.GetUserAgArch().Perceive());
            return true;
        }
    }
}
