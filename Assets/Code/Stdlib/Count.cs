using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: counts the number of occurrences of a particular belief
  (pattern) in the agent's belief base.
 */
namespace Assets.Code.Stdlib
{
    public class Count:DefaultInternalAction
    {
        public override int GetMinArgs()
        {
            return 2;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        protected override void CheckArguments(Term[] args)//: base.CheckArguments(args)
        {
            if (!(args[0].GetType() == typeof(LogicalFormula)))
            {
                throw JasonException.CreateWrongArgument(this, "first argument must be a formula");
            }
        }

        public override object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            LogicalFormula logExpr = args[0] as LogicalFormula;
            int n = 0;
            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(ts.GatAg(), un);
            while (iu.Current != null)
            {
                iu.MoveNext();
                n++;
            }
            return un.Unifies(args[1], new NumberTermImpl(n));
        }
    }
}
