using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
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

        protected override void CheckArguments(Term[] args): base.CheckArguments(args)
        {
            if (!args[0].IsLiteral())
            {
                throw JasonException.createWrongArgument(this, "first argument must be a literal");
            }
        }

        public override object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            return un.Unifies(args[1], args[0].MakeVarsAnnon() as Literal);
        }
    }
}
