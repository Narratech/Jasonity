using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;


namespace Assets.Code.functions
{
    public class ceil : ArithFunction
    {

        public override string GetName()
        {
            return "math.ceil";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsNumeric())
            {
                double n = ((INumberTerm)args[0]).Solve();
                return Math.Ceiling(n);
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