using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIMaAssets.Code.ReasoningCycle;
using BDIManager.Desires;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class SucceedGoalStdLib:InternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        protected override void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);
            if (!args[0].IsLiteral())
            {
                throw JasonityException.CreateWrongArgument(this, "first argument must be a literal");
            }
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            FindGoalAndDrop(reasoner, (Literal)args[0], un);
            return true;
        }

        public void FindGoalAndDrop(Reasoner rs, Literal l, Unifier un)
        {
            Trigger g = new Trigger(TEOperator.add, TEType.achieve, l);
            Circumstance C = rs.GetCircumstance();
            Unifier bak = un.Clone();

            IEnumerator<Intention> itinit = C.GetRunningIntentionsPlusAtomic();
            while (itinit.MoveNext())
            {
                Intention i = itinit.Current;
                if (DropGoal(i, g, rs, un) > 1)
                {
                    C.DropRunningIntention(i);
                    un = bak.Clone();
                }
            }

            // dropping the current intention?
            DropGoal(C.GetSelectedIntention(), g, rs, un);
            un = bak.Clone();

            //dropping G in Events
            IEnumerator<Event> ie = C.GetEventsPlusAtomic();
            while (ie.MoveNext())
            {
                Event e = ie.Current;
                //Test in the intention
                Intention i = e.GetIntention();
                int r = DropGoal(i, g, rs, un);
                if (r > 0)
                {
                    C.RemoveEvent(e);
                    if (r == 1)
                    {
                        C.ResumeIntention(i);
                    }
                    un = bak.Clone();
                }
                else
                {
                    //Test in the event
                    Trigger t = e.GetTrigger();
                    if (i != Intention.emptyInt && !i.IsFinished())
                    {
                        t = t.Capply(i.Peek().GetUnif());
                    }
                    if (un.Unifies(g, t))
                    {
                        DropGoalInEvent(rs, e, i);
                        un = bak.Clone();
                    }
                }
            }

            //dropping G in Pending Events
            foreach (string ek in C.GetPendingEvents().Keys)
            {
                //Test in the intention
                Event e = C.GetPendingEvents()[ek];
                Intention i = e.GetIntention();
                int r = DropGoal(i, g, rs, un);
                if (r > 0)
                {
                    C.RemovePendingEvent(ek);
                    if (r == 1)
                    {
                        C.ResumeIntention(i);
                    }
                    un = bak.Clone();
                }
                else
                {
                    //test in the event
                    Trigger t = e.GetTrigger();
                    if (i != Intention.emptyInt && !i.IsFinished())
                    {
                        t = t.Capply(i.Peek().GetUnif());
                    }
                    if (un.Unifies(g, t))
                    {
                        DropGoalInEvent(rs, e, i);
                        un = bak.Clone();
                    }
                }
            }

            //Dropping from pending Actions
            foreach (ExecuteAction a in C.GetPendingActions().Values)
            {
                Intention i = a.GetIntention();
                int r = DropGoal(i, g, rs, un);
                if (r > 0) //i was changed
                {
                    C.RemovePendingAction(i.GetID());   // remove i from PA
                    if (r == 1)                         // i must continue running
                    {
                        C.ResumeIntention(i);           // and put the intention back in I
                    }                                   // if r > 1, the event was generated and i will be back soon
                    un = bak.Clone();
                }
            }

            //Dropping from pending intentions
            foreach(Intention i in C.GetPendingIntentions().Values)
            {
                int r = DropGoal(i, g, rs, un);
                if (r > 0)
                {
                    C.RemovePendingIntention(i.GetID());
                    if (r == 1)
                    {
                        C.ResumeIntention(i);
                    }
                    un = bak.Clone();
                }
            }
        }

        /* returns: >0 the intention was changed
         *           1 = intention must continue running
         *           2 = fail event was generated and added in C.E
         *           3 = simply removed without event
         */
        public virtual int DropGoal(Intention i, Trigger g, Reasoner rs, Unifier un)
        {
            if (i != null && i.DropDesire(g, un))
            {
                if (rs.HasGoalListener())
                {
                    foreach (Desire gl in rs.GetDesiresListeners())
                    {
                        gl.DesireFinished(g, Desire.FinishStates.achieved);
                    }
                }
                //continue the intention
                if (!i.IsFinished())
                {
                    if (rs.GetCircumstance().GetSelectedIntention() != i)
                    {
                        i.Peek().RemoveCurrentStep();
                    }
                    rs.ApplyClrInt(i);
                    return 1;
                }
                else
                {
                    rs.ApplyClrInt(i);
                    return 3;
                }
            }
            return 0;
        }

        public virtual void DropGoalInEvent(Reasoner rs, Event e, Intention i)
        {
            Circumstance C = rs.GetCircumstance();
            C.RemoveEvent(e);
            if (i != null)
            {
                if (rs.HasGoalListener())
                {
                    foreach (Desire gl in rs.GetDesiresListeners())
                    {
                        gl.DesireFinished(e.GetTrigger(), Desire.FinishStates.achieved);
                    }
                    i.Peek().RemoveCurrentStep();
                    rs.ApplyClrInt(i);
                    C.AddRunningIntention(i);
                }
            }
        }
    }
}
