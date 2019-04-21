using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Code.Stdlib.ForkStdLib;

namespace Assets.Code.Stdlib
{
    public class JoinStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new JoinStdLib();
            return singleton;
        }

        override public ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray();
        }

        override protected void CheckArguments(ITerm[] args)
        {
        }

        override public bool SuspendIntention()
        {
            return true;
        }
        override public bool CanBeUsedInContext()
        {
            return false;
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            Intention currentInt = (Intention)reasoner.GetCircumstance().GetSelectedIntention();
            ForkData fd = (ForkData)((IObjectTerm)args[0]).GetObject();
            fd.toFinish--;

            // in the case of fork and, all intentions should be finished to continue
            if (fd.isAnd)
            {
                if (fd.toFinish == 0)
                {
                    currentInt.Peek().RemoveCurrentStep();
                    reasoner.GetCircumstance().AddRunningIntention(currentInt);
                }
            }
            else
            {
                // the first intention has finished, drop others
                fd.intentions.Remove(currentInt);
                foreach (Intention i in fd.intentions)
                {
                    //System.out.println("drop "+i.getId());
                    reasoner.GetCircumstance().DropIntention(i);
                }
                currentInt.Peek().RemoveCurrentStep();
                reasoner.GetCircumstance().AddRunningIntention(currentInt);
            }
            return true;
        }
    }
}
