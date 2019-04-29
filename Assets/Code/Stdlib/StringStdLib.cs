using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
  <p>Internal action: <b><code>.string</code></b>.
  <p>Description: checks whether the argument is a string, e.g.: "a".
  <p>Parameter:<ul>
  <li>+ arg[0] (any term): the term to be checked.<br/>
  </ul>
  <p>Examples:<ul>
  <li> <code>.string("home page")</code>: true.
  <li> <code>.string(b(10))</code>: false.
  <li> <code>.string(b)</code>: false.
  <li> <code>.string(X)</code>: false if X is free, true if X is bound to a string.
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class StringStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new StringStdLib();
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
            return args[0].IsString();
        }
    }
}
