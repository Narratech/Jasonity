using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action:
  <b><code>.resume(<i>G</i>)</code></b>.
  <p>Description: resume goals <i>G</i> that were suspended by <code>.suspend</code>.
  <br/>
  The meta-event <code>^!G[state(resumed)]</code> is produced.
  <p>Example:<ul>
  <li> <code>.resume(go(1,3))</code>: resume the goal of going to location 1,3.
  </ul>
 */

namespace Assets.Code.Stdlib
{
    public class ResumeStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 1;
        }
        override public int GetMaxArgs()
        {
            return 1;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[0].IsLiteral())
                throw JasonityException.CreateWrongArgument(this,"first argument must be a literal");
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            Trigger g = new Trigger(TEOperator.add, TEType.achieve, (Literal)args[0]);
            Circumstance C = ts.GetCircumstance();

            // Search the goal in PI
            IEnumerator<string> ik = C.GetPendingIntentions().KeySet().iterator();
            while (ik.hasNext()) {
                string k = ik.next();
                Intention i = C.GetPendingIntentions()[k];
                if (i.IsSuspended() && i.HasTrigger(g, un))
                {
                    i.SetSuspended(false);
                    bool notify = true;
                    if (k.StartsWith(SuspendStdLib.SUSPENDED_INT)) { // if not SUSPENDED_INT, it was suspended while already in PI, so, do not remove it from PI, just change the suspeded status
                        ik.remove();

                        // add it back in I if not in PA
                        if (! C.GetPendingActions().ContainsKey(i.GetID())) {
                            C.ResumeIntention(i);
                            notify = false; // the resumeIntention already notifies
                        }
                    }

                    // notify meta event listeners
                    if (notify && C.GetListeners() != null)
                        foreach (CircumstanceListener el in C.GetListeners())
                            el.intentionResumed(i);

                    // remove the IA .suspend in case of self-suspend
                    if (k.StartsWith(suspend.SELF_SUSPENDED_INT))
                        i.Peek().RemoveCurrentStep();

                    //System.out.println("res "+g+" from I "+i.getId());
                }
            }

            // Search the goal in PE
            ik = C.GetPendingEvents().KeySet().iterator();
            while (ik.hasNext()) {
                string k = ik.next();
                if (k.StartsWith(SuspendStdLib.SUSPENDED_INT)) {
                    Event e = C.GetPendingEvents()[k];
                    Intention i = e.GetIntention();
                    if (un.Unifies(g, e.GetTrigger()) || (i != null && i.HasTrigger(g, un))) {
                        ik.remove();
                        C.AddEvent(e);
                        if (i != null)
                            i.SetSuspended(false);
                    }
                }
            }
            return true;
        }
    }
}
