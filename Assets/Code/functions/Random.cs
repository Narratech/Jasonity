using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using Assets.Code.AsSemantics;

namespace Assets.Code.functions
{
    public partial class random : ArithFunction
    {

        public override string GetName()
        {
            return "math.random";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsNumeric() && args.Length == 1)
            {
                Random rnd = new Random();
                return rnd.Next() * ((INumberTerm)args[0]).Solve();
            }

            else
            {
                Random rnd = new Random();
                return rnd.Next();
            }
        }

        public override bool CheckArity(int a)
        {
            return a == 0 || a == 1;
        }
    }
}