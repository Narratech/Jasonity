using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System.Collections.Generic;

/*
 * Description: removes intentions to achieve goal <i>I</i> from the set of
 * intentions of the agent (suspended intentions are also considered).
 * No event is produced.
 */
namespace Assets.Code.Stdlib
{
    public class Drop_Intention: InternalAction
    {
        public override int GetMinArgs()
        {
            return 0;
        }

        public override int GetMaxArgs()
        {
            return 1;
        }

        protected override void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);
            if (args.Length > 0 && !args[0].IsLiteral() && !args[0].IsVar())
            {
                throw JasonityException.CreateWrongArgument(this, "first argument '" + args[0] + 
                    "' must be a literal or variable");
            }
        }

        private bool resultSuspend = false;

        public override bool SuspendIntention()
        {
            return resultSuspend;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            resultSuspend = false;
            if (args.Length == 0)
            {
                resultSuspend = true; // to drop the current intention
            }
            else
            {
                resultSuspend = DropInt(ts.GetCircumstance(), args[0] as Literal, un);
            }
            return true;
        }

        /**
         * Drops an intention based on a goal argument
         * 
         * returns true if the current intention is dropped
         */
        public bool DropInt(Circumstance C, Literal goal, Unifier un)
        {
            Unifier bak = un.Clone();
            Trigger g = new Trigger(TEOperator.add, TEType.achieve, goal);
            bool isCurrentInt = false;
            IEnumerator<Intention> iint = C.GetAllIntentions();
            while (iint.Current != null)
            {
                Intention i = iint.Current;
                if (i.HasTrigger(g, un))
                {
                    C.DropIntention(i);
                    isCurrentInt = isCurrentInt || i.Equals(C.GetSelectedIntention());
                    un = bak.Clone();
                }
            }
            return isCurrentInt;
        }
    }
}