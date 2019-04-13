using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;

namespace Assets.Code.functions
{
    public partial class random : DefaultArithFunction
    {

        public string GetName()
        {
            return "math.random";
        }

        public double Evaluate(Reasoner reasoner, ITerm[] args)
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

        public bool CheckArity(int a)
        {
            return a == 0 || a == 1;
        }
    }
}