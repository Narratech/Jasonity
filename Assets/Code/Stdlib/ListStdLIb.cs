using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.list</code></b>.
  <p>Description: checks whether the argument is a list, e.g.: "[a,b]", "[]".
  <p>Parameter:<ul>
  <li>+ argument (any term): the term to be checked.<br/>
  </ul>
  <p>Examples:<ul>
  <li> <code>.list([a,b,c])</code>: true.
  <li> <code>.list([a,b,c(X)])</code>: true.
  <li> <code>.list(b(10))</code>: false.
  <li> <code>.list(10)</code>: false.
  <li> <code>.list("home page")</code>: false.
  <li> <code>.list(X)</code>: false if X is free, true if X is bound to a list.
  <li> <code>.list(a(X))</code>: false.
  </ul>
*/
namespace Assets.Code.Stdlib
{
    public class ListStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new ListStdLib();
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
            return args[0].IsList();
        }
    }
}
