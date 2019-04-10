using Assets.Code.AsSyntax;

namespace Assets.Code.functions
{
    public partial class Count : DefaultArithFunction
    {

        public string GetName()
        {
            return ".count";
        }

        public double Evaluate(TransitionSystem ts, ITerm[] args)
        {
            if (ts == null)
            {
                throw new JasonException("The TransitionSystem parameter of the function '.count' cannot be null.");
            }
            ILogicalFormula logExpr = (ILogicalFormula)args[0];
            int n = 0;
            Iterator<Unifier> iu = logExpr.LogicalConsequence(ts.GetAg(), new Unifier());
            while (iu.HasNext())
            {
                iu.Next();
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