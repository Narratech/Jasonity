using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using Assets.Code.AsSemantics;

namespace Assets.Code.functions
{
    public class Min : ArithFunction
    {

        public override string GetName()
        {
            return "math.min";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsNumeric() && args[1].IsNumeric())
            {
                double n0 = ((INumberTerm)args[0]).Solve();
                double n1 = ((INumberTerm)args[0]).Solve();
                return Math.Min(n0, n1);
            }

            else if (args[0].IsList())
            {
                double min = Double.MaxValue;
                foreach (ITerm t in (IListTerm)args[0])
                {
                    if (t.IsNumeric())
                    {
                        double n = ((INumberTerm)t).Solve();
                        if (n < min)
                            min = n;
                    }
                }
                return min;
            }
            throw new JasonityException("The argument '" + args[0] + "' is not numeric!");
        }

        public override bool CheckArity(int a)
        {
            return a == 1 || a == 2;
        }
    }
}