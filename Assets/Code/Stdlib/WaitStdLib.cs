using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.ReasoningCycle;
using Assets.Code.Utilities;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.wait(<i>E</i>,<i>T</i>)</code></b>.
  <p>Description: suspend the intention for the time specified by <i>T</i> (in
  milliseconds) or until some event <i>E</i> happens. The events follow the
  AgentSpeak syntax but are enclosed by { and }, e.g. <code>{+bel(33)}</code>,
  <code>{+!go(X,Y)}</code>.
  <p>Parameters:<ul>
  <li><i>+ event</i> (trigger term [optional]): the event to wait for.<br/>
  <li><i>+ logical expression</i> ([optional]): the expression (as used on plans context) to wait to holds.<br/>
  <li>+ timeout (number [optional]): how many milliseconds should be waited.<br/>
  <li>- elapse time (var [optional]): the amount of time the intention was suspended waiting.<br/>
  </ul>
  <p>Examples:<ul>
  <li> <code>.wait(1000)</code>: suspend the intention for 1 second.
  <li> <code>.wait({+b(1)})</code>: suspend the intention until the belief
  <code>b(1)</code> is added in the belief base.
  <li> <code>.wait(b(X) & X > 10)</code>: suspend the intention until the agent believes
  <code>b(X)</code> with X greater than 10.
  <li> <code>.wait({+!g}, 2000)</code>: suspend the intention until the goal
  <code>g</code> is triggered or 2 seconds have passed, whatever happens
  first. In case the event does not happens in two seconds, the internal action
  fails.
  <li> <code>.wait({+!g}, 2000, EventTime)</code>: suspend the intention until the goal
  <code>g</code> is triggered or 2 seconds have passed, whatever happens
  first.
  As this use of .wait has three arguments, in case the event does not happen in
  two seconds, the internal action does not fail (as in the previous example).
  The third argument will be unified to the
  elapsed time (in milliseconds) from the start of .wait until the event or timeout. </ul>
 */

namespace Assets.Code.Stdlib
{
    public class WaitStdLib:InternalAction
    {
        public static readonly string waitAtom = ".wait";

        override public bool CanBeUsedInContext()
        {
            return false;
        }
        override public bool SuspendIntention()
        {
            return true;
        }

        override public int GetMinArgs()
        {
            return 1;
        }
        override public int GetMaxArgs()
        {
            return 3;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            long timeout = -1;
            Trigger te = null;
            ILogicalFormula f = null;
            ITerm elapsedTime = null;

            if (args[0].IsNumeric()) {
                // time in milliseconds
                INumberTerm time = (INumberTerm)args[0];
                timeout = (long) time.Solve();
            }
            else
            {
                te = Trigger.TryToGetTrigger(args[0]);   // wait for event
                if (te == null && args[0].GetType() == typeof(ILogicalFormula))
                {
                    // wait for an expression to become true
                    f = (ILogicalFormula) args[0];
                    if (ts.GetAgent().Believes(f, un))
                    { 
                        // if the agent already believes f
                        // place current intention back in I, since .wait usually does not do that
                        Intention si = ts.GetCircumstance().GetSelectedIntention();
                        si.Peek().RemoveCurrentStep();
                        ts.GetCircumstance().AddRunningIntention(si);
                        return true;
                    }
                }
                if (args.Length >= 2)
                    timeout = (long) ((INumberTerm) args[1]).Solve();
                if (args.Length == 3)
                    elapsedTime = args[2];
            }
            new WaitEvent(te, f, un, ts, timeout, elapsedTime);
            return true;
        }

        class WaitEvent : ICircumstanceListener
        {
            private Trigger te;
            private ILogicalFormula formula;
            private string sEvt; // a string version of what is being waited
            private Unifier un;
            private Intention si;
            private Reasoner ts;
            private Circumstance c;
            private bool dropped = false;
            private ITerm elapsedTimeTerm;
            private long startTime;

            public WaitEvent(Trigger te, ILogicalFormula f, Unifier un, Reasoner ts, long timeout, ITerm elapsedTimeTerm)
            {
                this.te = te;
                this.formula = f;
                this.un = un;
                this.ts = ts;
                c = ts.GetCircumstance();
                si = c.GetSelectedIntention();
                this.elapsedTimeTerm = elapsedTimeTerm;

                // register listener
                c.AddEventListener(this);

                if (te != null)
                {
                    sEvt = te.ToString();
                }
                else if (formula != null)
                {
                    sEvt = formula.ToString();
                }
                else
                {
                    sEvt = "time" + (timeout);
                }
                sEvt = si.GetID() + "/" + sEvt;
                c.AddPendingIntention(sEvt, si);

                startTime = System.currentTimeMillis(); //hay que usar el de c# o el de unity? MISTERIO

                if (timeout >= 0)
                {
                    //agent.getscheduler().schedule(new runnable()
                    //{
                    //    public void run()
                    //    {
                    //        resume(true);
                    //    }
                    //    }, timeout, timeunit.milliseconds);
                }
            }

            void Resume(bool stopByTimeout)
            {
                // unregister (to not receive intentionAdded again)
                c.RemoveEventListener(this);

                // invoke changes in C latter, so to avoid concurrent changes in C
                ts.RunAtBeginOfNextCycle(new RunnableImpl());
                /*{
                 * public void run() {
                    try {
                        // add SI again in C.I if (1) it was not removed (2) is is not running (by some other reason) -- but this test does not apply to atomic intentions --, and (3) this wait was not dropped
                        if (c.removePendingIntention(sEvt) == si && (si.isAtomic() || !c.hasRunningIntention(si)) && !dropped) {
                            if (stopByTimeout && (te != null || formula != null) && elapsedTimeTerm == null) {
                                // fail the .wait by timeout
                                if (si.isSuspended()) { // if the intention was suspended by .suspend
                                    PlanBody body = si.peek().getPlan().getBody();
                                    body.add(1, new PlanBodyImpl(BodyType.internalAction, new InternalActionLiteral(".fail")));
                                    c.addPendingIntention(suspend.SUSPENDED_INT+si.getId(), si);
                                } else {
                                    ts.generateGoalDeletion(si, JasonException.createBasicErrorAnnots("wait_timeout", "timeout in .wait"));
                                }
                            } else if (! si.isFinished()) {
                                si.peek().removeCurrentStep();

                                if (elapsedTimeTerm != null) {
                                    long elapsedTime = System.currentTimeMillis() - startTime;
                                    un.unifies(elapsedTimeTerm, new NumberTermImpl(elapsedTime));
                                }
                                if (si.isSuspended()) { // if the intention was suspended by .suspend
                                    c.addPendingIntention(suspend.SUSPENDED_INT+si.getId(), si);
                                } else {
                                    c.resumeIntention(si);
                                }
                            }
                        }
                    } catch (Exception e) {
                        ts.getLogger().log(Level.SEVERE, "Error at .wait thread", e);
                    }
}
                }*/
                ts.GetUserAgArch().WakeUpDeliberate();
            }

            public void EventAdded(Event e)
            {
                if (dropped)
                    return;
                if (te != null && un.Unifies(te, e.GetTrigger()))
                {
                    Resume(false);
                }
                else if (formula != null && ts.GetAgent().Believes(formula, un))
                { // each new event, just test the formula being waited
                    Resume(false);
                }
            }

            public void IntentionAdded(Intention i)
            {
                
            }

            public void IntentionDropped(Intention i)
            {
                if (i.Equals(si))
                {
                    dropped = true;
                    Resume(false);
                }
            }

            public void IntentionResumed(Intention i)
            {
                
            }

            public void IntentionSuspended(Intention i, string reason)
            {
                
            }

            public override string ToString()
            {
                return sEvt;
            }
        }

        class RunnableImpl : IRunnable
        {
            public void Run()
            {
                throw new NotImplementedException();
            }
        }
    }
}
