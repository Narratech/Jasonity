using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: evaluates the logical expression (which computes to true or false), the result is unified with Var
 */
namespace Assets.Code.Stdlib
{
    public class Eval:DefaultInternalAction
    {
        public int GetMinArgs()
        {
            return 2;
        }

        public int GetMaxArgs()
        {
            return 2;
        }

        public ITerm[] prepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray(); // we do not need clone neither apply for this internal action
        }

        protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);
            if (!(args[1].GetType() == typeof(ILogicalFormula)))
            {
                throw JasonityException.CreateWrongArgument(this, "second argument must be a logical formula");
            }
        }

        protected object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ILogicalFormula logExpr = args[1] as ILogicalFormula;
            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(ts.GetAgent(), un);
            if (iu.Current != null)
            {
                return un.Unifies(args[0], Literal.LTrue);
            }
            else
            {
                return un.Unifies(args[0], Literal.LFalse);
            }
        }
    }
}
