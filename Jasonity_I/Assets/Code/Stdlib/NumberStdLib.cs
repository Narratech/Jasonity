using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.number</code></b>.
  <p>Description: checks whether the argument is a number.
  <p>Parameter:<ul>
  <li>+ argument (any term): the term to be checked.<br/>
  </ul>
  <p>Examples:<ul>
  <li> <code>.number(10)</code>: true.
  <li> <code>.number(10.34)</code>: true.
  <li> <code>.number(b(10))</code>: false.
  <li> <code>.number("home page")</code>: false.
  <li> <code>.number(X)</code>: false if X is free, true if X is bound to a number.
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class NumberStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new NumberStdLib();
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
            return args[0].IsNumeric();
        }
    }
}
