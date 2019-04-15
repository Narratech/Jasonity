using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class IfThenElseStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new IfThenElseStdLib();
            }
            return singleton;
        }

        public override ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray();
        }

        protected override void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);
            if (!(args[0].GetType() == typeof(ILogicalFormula)))
                throw JasonityException.CreateWrongArgument(this, "first argument (test) must be a logical formula.");
            if (!args[1].IsPlanBody())
                throw JasonityException.CreateWrongArgument(this, "second argument (test) must be a plan body term.");
            if (args.Length == 3 && !args[2].IsPlanBody())
                throw JasonityException.CreateWrongArgument(this, "third argument (else) must be a plan body term.");
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            ILogicalFormula logExpr = (ILogicalFormula)args[0];
            IPlanBody whattoadd = null;

            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(reasoner.GetAgent(), un);
            if (iu.MoveNext())
            {
                whattoadd = (IPlanBody)args[1].Clone();
                un.Compose(iu.Current);
            }
            else if (args.Length == 3)
            {
                whattoadd = (IPlanBody)args[2].Clone();
            }

            if (whattoadd != null)
            {
                IntendedPlan ip = reasoner.GetCircumstance().GetSelectedIntention().Peek();
                whattoadd.Add(ip.GetCurrentStep().GetBodyNext());
                whattoadd.SetAsBodyTerm(false);
                ip.InsertAsNextStep(whattoadd);
            }
            return true;
        }
    }
}
