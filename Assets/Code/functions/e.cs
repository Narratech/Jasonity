using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;

namespace Assets.Code.functions
{
    public partial class e : ArithFunction
    {

        public override string GetName()
        {
            return "math.e";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            return Math.E;
        }

        public override bool CheckArity(int a)
        {
            return a == 0;
        }
    }
}