using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System.Collections.Generic;

namespace Assets.Code.functions
{
    public partial class Count : ArithFunction
    {

        public override string GetName()
        {
            return ".count";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (reasoner == null)
            {
                throw new JasonityException("The TransitionSystem parameter of the function '.count' cannot be null.");
            }
            ILogicalFormula logExpr = (ILogicalFormula)args[0];
            int n = 0;
            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(reasoner.GetAgent(), new Unifier());
            Unifier aux = new Unifier();
            while (iu.MoveNext())
            {
                aux = iu.Current;
                n++;
            }
            return n;
        }

        public override bool CheckArity(int a)
        {
            return a == 1;
        }

        public override bool AllowUngroundTerms()
        {
            return true;
        }
    }
}