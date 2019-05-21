using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class FindAllStdLib:InternalAction
    {
        public override int GetMinArgs()
        {
            return 3;
        }

        public override int GetMaxArgs()
        {
            return 3;
        }

        public override ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray(); // we do not need to clone nor to apply for this internal action
        }

        protected override void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!(args[1].GetType() == typeof(ILogicalFormula)))
            {
                throw JasonityException.CreateWrongArgument(this, "second argument must be a formula");
            }
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ITerm var = args[0];
            ILogicalFormula logExpr = (ILogicalFormula)args[1];
            IListTerm all = new ListTermImpl();
            IListTerm tail = all;
            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(reasoner.GetAgent(), un);
            while (iu.MoveNext())
            {
                tail = tail.Append(var.CApply(iu.Current));
            }
            return un.Unifies(args[2], all);
        }
    }
}
