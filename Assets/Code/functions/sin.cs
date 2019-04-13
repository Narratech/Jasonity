using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;

namespace Assets.Code.functions
{
    public partial class sin : ArithFunction
    {

        public override string GetName()
        {
            return "math.sin";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsNumeric())
            {
                double n = ((INumberTerm)args[0]).Solve();
                return Math.Sin(n);
            }
            else
            {
                throw new JasonityException("The argument '" + args[0] + "' is not numeric!");
            }
        }

        public override bool CheckArity(int a)
        {
            return a == 1;
        }
    }
}