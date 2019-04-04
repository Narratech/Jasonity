using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
using System.Collections.Generic;

/*
 * Description: removes intentions to achieve goal <i>I</i> from the set of
 * intentions of the agent (suspended intentions are also considered).
 * No event is produced.
 */
namespace Assets.Code.Stdlib
{
    public class Drop_Intention: DefaultInternalAction
    {
        public int GetMinArgs()
        {
            return 0;
        }

        public int GetMaxArgs()
        {
            return 1;
        }

        protected void CheckArguments(Term[] args)
        {
            base.CheckArguments(args);
            if (args.Length > 0 && !args[0].IsLiteral() && !args[0].IsVar())
            {
                throw JasonException.createWrongArgument(this, "first argument '" + args[0] + 
                    "' must be a literal or variable");
            }
        }

        private bool resultSuspend = false;

        public bool SuspendIntention()
        {
            return resultSuspend;
        }

        public object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            resultSuspend = false;
            if (args.Length == 0)
            {
                resultSuspend = true; // to drop the current intention
            }
            else
            {
                resultSuspend = DropInt(ts.GetC(), args[0] as Literal, un);
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
            Trigger g = new Trigger(TEOperator.Add, TeType.Achive, goal);
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