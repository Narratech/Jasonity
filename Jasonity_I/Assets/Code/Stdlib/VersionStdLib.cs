using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.version(<i>V</i>)</code></b>.
  <p>Description: unifies <i>V</i> with the Jason version.
  <p>Parameter:<ul>
  <li>- version (string): the variable to receive the version<br/>
  </ul>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class VersionStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new VersionStdLib();
            return singleton;
        }


        override public int GetMinArgs()
        {
            return 1;
        }
        override public int GetMaxArgs()
        {
            return 1;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            //return un.Unifies(args[0], new StringTermImpl(Config.Get().GetJasonVersion()));
            return un.Unifies(args[0], new StringTermImpl("Version 1.0"));
        }
    }
}