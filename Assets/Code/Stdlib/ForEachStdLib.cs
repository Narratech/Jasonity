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
    public class ForEachStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
                singleton = new ForEachStdLib();
            return singleton;
        }

        public override ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray();
        }

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
            base.CheckArguments(args); // check number of arguments
            if ( !(args[0].GetType() == typeof(ILogicalFormula)))
                throw JasonityException.CreateWrongArgument(this,"first argument must be a logical formula.");
            if ( !args[1].IsPlanBody())
                throw JasonityException.CreateWrongArgument(this,"second argument must be a plan body term.");
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            IntendedPlan im = reasoner.GetCircumstance().GetSelectedIntention().Peek();
            IPlanBody foria = im.GetCurrentStep();

            IEnumerator<Unifier> iu;

            if (args.Length == 2)
            {
                // first execution of while
                CheckArguments(args);

                // get all solutions for the loop
                // Note: you should get all solutions here, otherwise a concurrent modification will occur for the iterator
                ILogicalFormula logExpr = (ILogicalFormula)args[0];
                iu = logExpr.LogicalConsequence(reasoner.GetAgent(), un);
                List<Unifier> allsol = new List<Unifier>();
                while (iu.MoveNext())
                    allsol.Add(iu.Current);
                if (allsol.Count == 0)
                    return true;
                iu = allsol.GetEnumerator();
                foria = new PlanBodyImpl(BodyType.internalAction, foria.GetBodyTerm().Clone());
                foria.Add(im.GetCurrentStep().GetBodyNext());
                Structure forstructure = (Structure)foria.GetBodyTerm();
                forstructure.AddTerm(new ObjectTermImpl(iu));         // store all solutions
                forstructure.AddTerm(new ObjectTermImpl(un.Clone())); // backup original unifier
            }
            else if (args.Length == 4)
            {
                // restore the solutions
                iu = (IEnumerator<Unifier>)((IObjectTerm)args[2]).GetObject();
            }
            else
            {
                throw JasonityException.CreateWrongArgumentNb(this);
            }

            un.Clear();
            if (iu.MoveNext())
            {
                // add in the current intention:
                // 1. the body argument of for and
                // 2. the for internal action after the execution of the body
                //    (to perform the next iteration)
                un.Compose(iu.Current);
                IPlanBody whattoadd = (IPlanBody)args[1].Clone();
                whattoadd.Add(foria);
                whattoadd.SetAsBodyTerm(false);
                im.InsertAsNextStep(whattoadd);
            }
            else
            {
                un.Compose((Unifier)((IObjectTerm)args[3]).GetObject());
            }
            return true;
        }
    }
}
