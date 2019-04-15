
using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.literal</code></b>.
  <p>Description: checks whether the argument is a literal,
  e.g.: "p", "p(1)", "p(1)[a,b]", "~p(1)[a,b]".
  <p>Parameter:<ul>
  <li>+ argument (any term): the term to be checked.<br/>
  </ul>
  <p>Examples:<ul>
  <li> <code>.literal(b(10))</code>: true.
  <li> <code>.literal(b)</code>: true.
  <li> <code>.literal(10)</code>: false.
  <li> <code>.literal("Jason")</code>: false.
  <li> <code>.literal(X)</code>: false if X is free, true if X is bound to a literal.
  <li> <code>.literal(a(X))</code>: true.
  <li> <code>.literal([a,b,c])</code>: false.
  <li> <code>.literal([a,b,c(X)])</code>: false.
  </ul>
*/
namespace Assets.Code.Stdlib
{
    public class LiteralStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new LiteralStdLib();
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

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return args[0].IsLiteral();
        }
    }
}
