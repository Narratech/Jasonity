using Assets.Code.AsSemantics;
using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIMaAssets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action:
  <b><code>.suspend(<i>G</i>)</code></b>.
  <p>Description: suspend goals <i>G</i>, i.e., all intentions trying to achieve G will stop
  running until the internal action <code>.resume</code> change the state of those intentions.
  A literal <i>G</i>
  is a goal if there is a triggering event <code>+!G</code> in any plan within
  any intention in I, E, PI, or PA.
  <br/>
  The meta-event <code>^!G[state(suspended)]</code> is produced.
  <p>Examples:<ul>
  <li> <code>.suspend(go(1,3))</code>: suspends intentions to go to the location 1,3.
  <li> <code>.suspend</code>: suspends the current intention.
  </ul>
 */

namespace Assets.Code.Stdlib
{
    public class SuspendStdLib:InternalAction
    {
        bool suspendIntention = false;
        public static readonly string SUSPENDED_INT      = "suspended-";
        public static readonly string SELF_SUSPENDED_INT = SUSPENDED_INT+"self-";

        override public int GetMinArgs()
        {
            return 0;
        }
        override public int GetMaxArgs()
        {
            return 1;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (args.Length == 1 && !args[0].IsLiteral())
                throw JasonityException.CreateWrongArgument(this,"first argument must be a literal");
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            suspendIntention = false;

            Circumstance C = ts.GetCircumstance();

            if (args.Length == 0)
            {
                // suspend the current intention
                Intention i = C.GetSelectedIntention();
                suspendIntention = true;
                i.SetSuspended(true);
                C.AddPendingIntention(SELF_SUSPENDED_INT + i.GetID(), i);
                return true;
            }

            // use the argument to select the intention to suspend.

            Trigger g = new Trigger(TEOperator.add, TEType.achieve, (Literal)args[0]);

            // ** Must test in PA/PI first since some actions (as .suspend) put intention in PI

            // suspending from Pending Actions
            foreach (ExecuteAction a in C.GetPendingActions().Values)
            {
                Intention ia = a.GetIntention();
                if (ia.HasTrigger(g, un))
                {
                    ia.SetSuspended(true);
                    C.AddPendingIntention(SUSPENDED_INT + ia.GetID(), ia);
                }
            }

            // suspending from Pending Intentions
            foreach (Intention ii in C.GetPendingIntentions().Values)
            {
                if (ii.HasTrigger(g, un))
                {
                    ii.SetSuspended(true);
                }
            }

            IEnumerator<Intention> itint = C.GetRunningIntentionsPlusAtomic();
            while (itint.MoveNext())
            {
                Intention i = itint.Current;
                if (i.HasTrigger(g, un))
                {
                    i.SetSuspended(true);
                    C.RemoveRunningIntention(i);
                    C.AddPendingIntention(SUSPENDED_INT + i.GetID(), i);
                }
            }

            // suspending the current intention? <-(Esta interrogación ya venía, lo juro)
            Intention ci = C.GetSelectedIntention();
            if (ci != null && ci.HasTrigger(g, un))
            {
                suspendIntention = true;
                ci.SetSuspended(true);
                C.AddPendingIntention(SELF_SUSPENDED_INT + ci.GetID(), ci);
            }

            // suspending G in Events
            int c = 0;
            IEnumerator<Event> ie = C.GetEventsPlusAtomic();
            while (ie.MoveNext())
            {
                Event e = ie.Current;
                ci = e.GetIntention();
                if (un.Unifies(g, e.GetTrigger()) || (ci != null && ci.HasTrigger(g, un)))
                {
                    C.RemoveEvent(e);
                    C.AddPendingEvent(SUSPENDED_INT + e.GetTrigger() + (c++), e);
                    if (ci != null)
                        ci.SetSuspended(true);
                }
            }
            return true;
        }
    }
}
