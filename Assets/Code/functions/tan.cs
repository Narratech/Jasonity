using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;

namespace Assets.Code.functions
{
    public partial class tan : DefaultArithFunction
    {

        public string GetName()
        {
            return "math.tan";
        }

        public double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsNumeric())
            {
                double n = ((INumberTerm)args[0]).Solve();
                return Math.Tan(n);
            }
            else
            {
                throw new JasonException("The argument '" + args[0] + "' is not numeric!");
            }
        }

        public bool CheckArity(int a)
        {
            return a == 1;
        }
    }
}