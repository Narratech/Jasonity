using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.upper_case(S1,S2)</code></b>.
  <p>Description: converts the string S1 into upper case S2.
  <p>Parameters:<ul>
  <li>+ S1 (a term). The term representation as a string will be used.<br/>
  <li>-/+ S2 (a string).<br/>
  </ul>
  <p>Examples:<ul>
  <li> <code>.upper_case("CArtAgO",X)</code>: unifies X with "CARTAGO".
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class UpperCaseStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new UpperCaseStdLib();
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
            string arg = null;
            if (args[0].IsString())
                arg = ((IStringTerm) args[0]).GetString();
            else
                arg = args[0].ToString();
            arg = arg.ToUpper();
            return un.Unifies(new StringTermImpl(arg), args[1]);
        }
    }
}
