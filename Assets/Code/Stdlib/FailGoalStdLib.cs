using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using static BDIManager.Desires.DesireStdlib;

namespace Assets.Code.Stdlib
{
    public class FailGoalStdLib: SucceedGoalStdLib
    {
        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            FindGoalAndDrop(reasoner, (Literal)args[0], un);
            return true;
        }

        /* returns: >0 the intention was changed
         *           1 = intention must continue running
         *           2 = fail event was generated and added in C.E
         *           3 = simply removed without event
         */
        public override int DropGoal(Intention i, Trigger g, Reasoner rs, Unifier un)
        {
            if (i != null)
            {
                if (i.DropGoal(g ,un))
                {
                    //notify listener
                    if (rs.HasGoalListener())
                    {
                        foreach (GoalListener gl in rs.GetGoalListeners())
                        {
                            gl.GoalFailed(g);
                        }
                    }

                    //generate failure event
                    Event failEvent = TaskScheduler.FindEventForFailure(i, g); //find fail event for the goal just dropped
                    if (failEvent != null)
                    {
                        failEvent = new Event(failEvent.GetTrigger().Capply(un), failEvent.GetIntention());
                        rs.GetCircumstance().AddEvent(failEvent);
                        return 2;
                    }
                    else //i is finished or without failure plan
                    {
                        if (rs.HasGoalListener())
                        {
                            foreach (GoalListener gl in rs.GetGoalListeners())
                            {
                                gl.GoalFinished(g, FinishStates.unachieved);
                            }
                        }
                        i.Fail(rs.GetCircumstance());
                        return 3;
                    }
                }
            }
            return 0;
        }

        public override void DropGoalInEvent(Reasoner rs, Event e, Intention i)
        {
            e.GetTrigger().SetTrigOp(TEOperator.del);
        }
    }
}
