using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using Assets.Code.AsSemantics;

namespace Assets.Code.functions
{
    public partial class Sum : ArithFunction
    {

        public override string GetName()
        {
            return "math.sum";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsList())
            {
                double sum = 0;

                foreach (ITerm t in (IListTerm)args[0])
                {
                    if (t.IsNumeric())
                    {
                        sum += ((INumberTerm)t).Solve();
                    }
                }
                return sum;
            }
            throw new JasonityException(GetName() + " is not implemented for type '" + args[0] + "'.");
        }

        public override bool CheckArity(int a)
        {
            return a == 1;
        }
    }
}