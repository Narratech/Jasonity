using Assets.Code.AsSyntax;

namespace Assets.Code.functions
{
    public partial class average : DefaultArithFunction
    {

        public string GetName()
        {
            return "math.average";
        }

        public double Evaluate(TransitionSystem ts, ITerm[] args)
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
            throw new JasonException(GetName() + " is not implemented for type '" + args[0] + "'.");
        }

        public bool CheckArity(int a)
        {
            return a == 1;
        }
    }
}