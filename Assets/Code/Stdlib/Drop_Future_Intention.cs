using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Drop_Future_Intention:Drop_Desire
    {
        public int GetMinArgs()
        {
            return 1;
        }

        public int GetMaxArgs()
        {
            return 1;
        }

        protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);
            if (!args[0].IsLiteral())
            {
                throw JasonException.createWrongArgument(this, "first argument '" + args[0] + "' must be a literal");
            }
        }

        public bool DropInt(Circumstance C, Literal goal, Unifier un)
        {
            Unifier bak = un.Clone();
            bool isCurrentInt = false;
            IEnumerator<Intention> iint = C.GetAllIntentions();
            while (iint.Current != null)
            {
                Intention i = iint.Current;
                PlanBody pb = i.Peek().GetPlan().GetBody();
                while (pb != null)
                {
                    if (pb.GetBodyType() == BodyType.Achive || pb.GetBodyType() == BodyType.AchiveNF)
                    {
                        if (un.Unifies(pb.GetBodyTerm(), goal))
                        {
                            C.DropIntention(i);
                            isCurrentInt = isCurrentInt || i.Equals(C.GetSelectedIntention());
                            un = bak.Clone();
                            break;
                        }
                    }
                    pb = pb.GetBodyNext();
                }
            }
            return isCurrentInt;
        }
    }
}
