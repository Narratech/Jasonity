using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>replace the variable by unused named, to avoid clash.
  <p>Examples:<ul>
  <li> <code>.rename_apart(b(X,Y,a), R)</code>: R will unifies with
  <code>b(_33_X,_34_Y,a)</code>.</li>
  </ul>
 */

namespace Assets.Code.Stdlib
{
    public class RenameApartStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 2;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        override public ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray();
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            Literal newl = (Literal)args[0];
            if (newl.IsVar()) { // does 1 step unification
            Literal vl = (Literal)un.Get((VarTerm)newl);
            if (vl != null)
                newl = vl;
            }
            newl = newl.MakeVarsAnnon();
            return un.Unifies(args[1], newl);
        }
    }
}
