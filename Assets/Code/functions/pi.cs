using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;

namespace Assets.Code.functions
{
    public partial class pi : DefaultArithFunction
    {

        public string GetName()
        {
            return "math.pi";
        }

        public double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            return Math.PI;
        }

        public bool CheckArity(int a)
        {
            return a == 0;
        }
    }
}