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
  <p>Internal action: <b><code>.reverse</code></b>.
  <p>Description: reverses strings or lists.
  <p>Parameters:<ul>
  <li>+ arg[0] (list or string): the string or list to be reversed.<br/>
  <li>+/- arg[1]: the result.
  </ul>
  <p>Examples:<ul>
  <li> <code>.reverse("abc",X)</code>: <code>X</code> unifies with "cba".
  <li> <code>.reverse("[a,b,c]",X)</code>: <code>X</code> unifies with "[c,b,a]".
  <li> <code>.reverse("[a,b,c|T]",X)</code>: <code>X</code> unifies with "[c,b,a|T]".
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class ReverseStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new ReverseStdLib();
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

            if (args[0].IsList()) {
            // list reverse
            if (!args[1].IsVar() && !args[1].IsList())
                throw JasonityException.CreateWrongArgument(this,"last argument '"+args[1]+"' must be a list or a variable.");

                return un.Unifies(((IListTerm) args[0]).Reverse(), args[1]);

            }
            else {
                // string reverse
                if (!args[1].IsVar() && !args[1].IsString())
                    throw JasonityException.CreateWrongArgument(this,"last argument '"+args[1]+"' must be a string or a variable.");
                string vl = args [0].ToString();
                if (args [0].IsString())
                vl = ((IStringTerm)args [0]).GetString();

                /*All this shit it's because c#'s StringBuilder doesn't have reverse method in */
                char[] charArray = new StringBuilder(vl).ToString().ToCharArray();
                Array.Reverse(charArray);
                /********************************************************************/
                return un.Unifies(new StringTermImpl(new string(charArray)/*new StringBuilder(vl).Reverse().ToString()*/), args[1]);
            }
        }
    }
}
