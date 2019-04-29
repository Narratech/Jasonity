using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using Assets.Code.AsSemantics;

namespace Assets.Code.functions
{
    public partial class Max : ArithFunction
    {

        public override string GetName()
        {
            return "math.max";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsNumeric() && args[1].IsNumeric())
            {
                double n0 = ((INumberTerm)args[0]).Solve();
                double n1 = ((INumberTerm)args[0]).Solve();
                return Math.Max(n0, n1);
            }

            else if (args[0].IsList()) 
            {
                double max = Double.MinValue;
                foreach (ITerm t in (IListTerm)args[0])
                {
                    if (t.IsNumeric())
                    {
                        double n = ((INumberTerm)t).Solve();
                        if (n > max)
                            max = n;
                    }
                }
                return max;
            }
            throw new JasonityException("The argument '" + args[0] + "' is not numeric!");
        }

        public override bool CheckArity(int a)
        {
            return a == 1 || a == 2;
        }
    }
}