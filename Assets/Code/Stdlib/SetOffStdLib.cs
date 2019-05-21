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
    public class SetOffStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 3;
        }
        override public int GetMaxArgs()
        {
            return 3;
        }

        override public ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray(); // we do not need to clone nor to apply for this internal action
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!(args[1].GetType() == typeof(ILogicalFormula)))
                throw JasonityException.CreateWrongArgument(this,"second argument must be a formula");
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            ITerm var = args[0];
            ILogicalFormula logExpr = (ILogicalFormula)args[1];
            ISet<ITerm> all = new HashSet<ITerm>();
            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(ts.GetAgent(), un);
            while (iu.MoveNext()) {
                all.Add(var.CApply(iu.Current));
            }
            return un.Unifies(args[2], SetToList(all));
        }

        // copy the set to a new list
        private IListTerm SetToList(ISet<ITerm> set)
        {
            IListTerm result = new ListTermImpl();
            IListTerm tail = result;
            foreach (ITerm t in set)
                tail = tail.Append(t.Clone());
            return result;
        }
    }
}
