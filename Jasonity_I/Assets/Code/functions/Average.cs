using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;


namespace Assets.Code.functions
{
    public class Average : ArithFunction
    {

        public override string GetName()
        {
            return "math.average";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsList())
            {
                double sum = 0;
                int n = 0;
                
                foreach (ITerm t in (IListTerm)args[0])
                {
                    if (t.IsNumeric())
                    {
                        sum += ((INumberTerm)t).Solve();
                        n++;
                    }
                }
                return sum / n;
            }
            throw new JasonityException(GetName() + " is not implemented for type '" + args[0] + "'.");
        }

        public override bool CheckArity(int a)
        {
            return a == 1;
        }
    }
}