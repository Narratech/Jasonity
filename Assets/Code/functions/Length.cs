using Assets.Code.AsSyntax;
using System;

namespace Assets.Code.functions
{
    public partial class Length : DefaultArithFunction
    {

        public string GetName()
        {
            return ".length";
        }

        public double Evaluate(TransitionSystem ts, ITerm[] args)
        {
            if (args[0].IsList())
            {
                return ((IListTerm)args[0]).Size();
            }
            else if (args[0].IsString())
            {
                return ((IStringTerm)args[0]).GetString().Length();
            }
            else
            {
                throw new JasonException("The argument '" + args[0] + "' is not numeric!");
            }
        }

        public bool CheckArity(int a)
        {
            return a == 1;
        }
    }
}