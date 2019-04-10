using Assets.Code.AsSyntax;
using System;

namespace Assets.Code.functions
{
    public partial class StdDev : DefaultArithFunction
    {

        public string GetName()
        {
            return "math.std_dev";
        }

        public double Evaluate(TransitionSystem ts, ITerm[] args)
        {
            if (args[0].IsList())
            {
                double sum = 0, squareSum = 0, num;
                int n = 0;

                foreach (ITerm t in (IListTerm)args[0])
                    if (t.IsNumeric())
                    {
                        if (t.IsNumeric())
                        {
                            num = ((INumberTerm)t).Solve();
                            sum += num;
                            n++;
                        }
                    }
                double mean = sum / n;

                foreach (ITerm t in (IListTerm)args[0])
                    if (t.IsNumeric())
                    {
                        num = ((INumberTerm)t).Solve();
                        squareSum += (num - mean) * (num - mean);
                    }
                return Math.Sqrt(squareSum / (n - 1));
            }
            throw new JasonException(GetName() + " is not implemented for type '" + args[0] + "'.");
        }

        public bool CheckArity(int a)
        {
            return a == 1;
        }
    }
}