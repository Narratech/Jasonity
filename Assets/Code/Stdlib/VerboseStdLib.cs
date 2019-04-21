using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.verbose</code></b>.
  <p>Description: change the verbosity level.
  <p>Parameters:<ul>
  <li>+value (number): values are 0 (minimal), 1 (normal) and 2 (debug).<br/>
  </ul>
  <p>Example:<ul>
  <li> <code>.verbose(2)</code>: start showing debug messages</li>
  <li> <code>.verbose(1)</code>: show 'normal' messages</li>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class VerboseStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new VerboseStdLib();
            return singleton;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            Settings stts = ts.GetSettings();
            stts.SetVerbose((int)((INumberTerm) args[0]).Solve());
            ts.GetAgent().GetLogger().SetLevel(stts.LogLevel());
            ts.GetLogger().SetLevel(stts.LogLevel());
            return true;
        }
    }
}
