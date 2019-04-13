using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: returns a description of the current intention. It is useful
  for plans that need to inspect the current intention.
 */
namespace Assets.Code.Stdlib
{
    public class Current_Intention:DefaultInternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 1;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            //try to get the intention from the "body"
            Intention i = ts.GetCircumstance().GetSelectedIntention();

            if (i == null)
            {
                //try to get the intention from the event
                Event evt = ts.GetCircumstance().GetSelectedEvent();
                if (evt != null)
                {
                    i = evt.GetIntention();
                }
                if (i != null)
                {
                    return un.Unifies(i.GetAsTerm(), args[0]);
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
