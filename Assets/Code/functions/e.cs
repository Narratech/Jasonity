using Assets.Code.AsSyntax;
using System;

namespace Assets.Code.functions
{
    public partial class e : DefaultArithFunction
    {

        public string GetName()
        {
            return "math.e";
        }

        public double Evaluate(TransitionSystem ts, ITerm[] args)
        {
            return Math.E;
        }

        public bool CheckArity(int a)
        {
            return a == 0;
        }
    }
}