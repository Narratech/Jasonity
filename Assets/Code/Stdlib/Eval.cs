using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
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

        public Term[] prepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray(); // we do not need clone neither apply for this internal action
        }

        protected void CheckArguments(Term[] args)
        {
            base.CheckArguments(args);
            if (!(args[1].GetType() == typeof(LogicalFormula)))
            {
                throw JasonException.createWrongArgument(this, "second argument must be a logical formula");
            }
        }

        protected object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            LogicalFormula logExpr = args[1] as LogicalFormula;
            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(ts.GetAg(), un);
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
