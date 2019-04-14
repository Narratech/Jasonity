using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using Assets.Code.AsSemantics;

namespace Assets.Code.functions
{
    public partial class acos : ArithFunction
    {

        public override string GetName()
        {
            return "math.acos";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsNumeric())
            {
                double n = ((INumberTerm)args[0]).Solve();
                return Math.Acos(((INumberTerm)args[0]).Solve());
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