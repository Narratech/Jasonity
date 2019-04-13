using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;

namespace Assets.Code.functions
{
    public partial class e : DefaultArithFunction
    {

        public string GetName()
        {
            return "math.e";
        }

        public double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            return Math.E;
        }

        public bool CheckArity(int a)
        {
            return a == 0;
        }
    }
}