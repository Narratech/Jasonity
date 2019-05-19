
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;

namespace Assets.Code.functions
{
    //partial 
    public class Abs : ArithFunction
    {

        public override string GetName()
        {
            return "math.abs";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsNumeric())
            {
                double n = ((INumberTerm)args[0]).Solve();
                return Math.Abs(n);
            }
            else
            {
                throw new JasonityException("The argument '"+args[0]+"' is not numeric!");
            }
        }

        public override bool CheckArity(int a)
        {
            return a == 1;
        }
    }
}