using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System.Collections.Generic;

namespace Assets.Code.functions
{
    public partial class Count : DefaultArithFunction
    {

        public string GetName()
        {
            return ".count";
        }

        public double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (reasoner == null)
            {
                throw new JasonException("The TransitionSystem parameter of the function '.count' cannot be null.");
            }
            ILogicalFormula logExpr = (ILogicalFormula)args[0];
            int n = 0;
            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(reasoner.GetAgent(), new Unifier());
            while (iu.MoveNext())
            {
                iu.Current;
                n++;
            }
            return n;
        }

        public bool CheckArity(int a)
        {
            return a == 1;
        }

        public bool AllowUngroundTerms()
        {
            return true;
        }
    }
}