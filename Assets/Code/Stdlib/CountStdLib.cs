using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
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
    public class CountStdLib:InternalAction
    {
        public override int GetMinArgs()
        {
            return 2;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        protected override void CheckArguments(ITerm[] args)//: base.CheckArguments(args)
        {
            if (!(args[0].GetType() == typeof(ILogicalFormula)))
            {
                throw JasonityException.CreateWrongArgument(this, "first argument must be a formula");
            }
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ILogicalFormula logExpr = args[0] as ILogicalFormula;
            int n = 0;
            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(ts.GetAgent(), un);
            while (iu.Current != null)
            {
                iu.MoveNext();
                n++;
            }
            return un.Unifies(args[1], new NumberTermImpl(n));
        }
    }
}
