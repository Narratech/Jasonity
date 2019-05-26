using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Code.AsSyntax.PlanBodyImpl;

namespace Assets.Code.Stdlib
{
    public class DropFutureIntentionStdLib:DropDesireStdLib
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
            if (!args[0].IsLiteral())
            {
                throw JasonityException.CreateWrongArgument(this, "first argument '" + args[0] + "' must be a literal");
            }
        }

        public override bool DropInt(Circumstance C, Literal goal, Unifier un)
        {
            Unifier bak = un.Clone();
            bool isCurrentInt = false;
            IEnumerator<Intention> iint = C.GetAllIntentions();
            while (iint.Current != null)
            {
                Intention i = iint.Current;
                IPlanBody pb = i.Peek().GetPlan().GetBody();
                while (pb != null)
                {
                    if (pb.GetBodyType() == BodyType.Body_Type.achieve || pb.GetBodyType() == BodyType.Body_Type.achieveNF)
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
