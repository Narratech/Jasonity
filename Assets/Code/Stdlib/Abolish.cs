using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * Description: removes all beliefs that match the argument
 */
namespace Assets.Code.Stdlib
{
    public class Abolish : InternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 1;
        }

        protected override void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);
            if (!args[0].IsLiteral() && !args[0].IsVar())
            {
                throw JasonityException.CreateWrongArgument(this, "first argument must be a literal or variable.");
            }
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ts.GetAgent().Abolish((Literal)args[0], un);
            return true;
        }
    }
}
