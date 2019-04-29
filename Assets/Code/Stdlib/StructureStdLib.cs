using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.structure</code></b>.
  <p>Description: checks whether the argument is a structure, e.g.: "p", "p(1)",
  "[a,b]".  Numbers, strings and free variables are not structures.
  <p>Parameter:<ul>
  <li>+ argument (any term): the term to be checked.<br/>
  </ul>
  <p>Examples:<ul>
  <li> <code>.structure(b(10))</code>: true.
  <li> <code>.structure(b)</code>: true.
  <li> <code>.structure(10)</code>: false.
  <li> <code>.structure("home page")</code>: false.
  <li> <code>.structure(X)</code>: false if X is free, true if X is bound to a structure.
  <li> <code>.structure(a(X))</code>: true.
  <li> <code>.structure([a,b,c])</code>: true.
  <li> <code>.structure([a,b,c(X)])</code>: true.
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class StructureStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new StructureStdLib();
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
            return args[0].IsStructure();
        }
    }
}
