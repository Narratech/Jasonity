using Assets.Code.AsSyntax;
using System;

namespace Assets.Code.functions
{
    public partial class pi : DefaultArithFunction
    {

        public string GetName()
        {
            return "math.pi";
        }

        public double Evaluate(TransitionSystem ts, ITerm[] args)
        {
            return Math.PI;
        }

        public bool CheckArity(int a)
        {
            return a == 0;
        }
    }
}