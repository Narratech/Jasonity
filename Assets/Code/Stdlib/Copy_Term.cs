using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Copy_Term: DefaultInternalAction
    {
        public override int GetMinArgs()
        {
            return 2;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        protected override void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);
            if (!args[0].IsLiteral())
            {
                throw JasonityException.createWrongArgument(this, "first argument must be a literal");
            }
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            return un.Unifies(args[1], args[0].MakeVarsAnnon() as Literal);
        }
    }
}
