using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;

namespace Assets.Code.functions
{
    public partial class Length : ArithFunction
    {

        public override string GetName()
        {
            return ".length";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            if (args[0].IsList())
            {
                return ((IListTerm)args[0]).Size();
            }
            else if (args[0].IsString())
            {
                return ((IStringTerm)args[0]).GetString().Length;
            }
            else
            {
                throw new JasonityException("The argument '" + args[0] + "' is not numeric!");
            }
        }

        public override bool CheckArity(int a)
        {
            return a == 1;
        }
    }
}