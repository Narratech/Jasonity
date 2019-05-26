using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.term2string(T,S)</code></b>.
  <p>Description: converts the term T into a string S and vice-versa.
  <p>Parameters:<ul>
  <li>-/+ T (any term).<br/>
  <li>-/+ S (a string).<br/>
  </ul>
  <p>Examples:<ul>
  <li> <code>.term2string(b,"b")</code>: true.
  <li> <code>.term2string(b,X)</code>: unifies X with "b".
  <li> <code>.term2string(X,"b")</code>: unifies X with b.
  <li> <code>.term2string(X,"10")</code>: unifies X with 10 (a number term).
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class Terms2StringStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new Terms2StringStdLib();
            return singleton;
        }

        override public int GetMinArgs()
        {
            return 2;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            // case 1, no vars
            if (!args[0].IsVar() && args[1].IsString()) {
                return args[0].ToString().Equals(((IStringTerm) args[1]).GetString() );
            }

            // case 2, second is var
            if (!args[0].IsVar() && args[1].IsVar()) {
                return un.Unifies(new StringTermImpl(args[0].ToString()), args[1]);
            }

            // case 3, first is var
            if (args[0].IsVar()) {
                if (args[1].IsString()) {
                    return un.Unifies(args[0], AsSyntax.AsSyntax.ParseTerm(((IStringTerm) args[1]).GetString() ));
                } else {
                    return un.Unifies(args[0], AsSyntax.AsSyntax.ParseTerm(args[1].ToString()));                
                }
            }

            throw new JasonityException("invalid case of term2string");
        }
    }
}
