using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using Assets.Code.AsSemantics;

namespace Assets.Code.functions
{
    public partial class pi : ArithFunction
    {

        public override string GetName()
        {
            return "math.pi";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            return Math.PI;
        }

        public override bool CheckArity(int a)
        {
            return a == 0;
        }
    }
}