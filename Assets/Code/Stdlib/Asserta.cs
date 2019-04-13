using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Beliefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Asserta: InternalAction
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
                if (!args[0].IsGround() && !args[0].IsRule())
                {
                    throw JasonityException.CreateWrongArgument(this, "first argument must be a ground literal (or rule).");
                }
            }
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            Literal l = (Literal)args[0];
            if (!l.HasSource())
            {
                l.AddAnnot(BeliefBase.TSelf);
            }
            List<Literal>[] result = ts.GetAgent().Brf(l, null, null, false);
            if (result != null)
            {
                ts.UpdateEvents(result, null);
            }
            return true;
        }
    }
}
