using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jason.AsSemantics.AsSemantics
{
    public class GoalListenerForMetaEvents : GoalListener
    {
        private TransitionSystem ts;

        public void goalFailed(Trigger goal)
        {
            generateGoalStateEvent(goal.getLiteral(), goal.getType(), GoalStates.failed, null);
        }

        public void goalFinished(Trigger goal, FinishStates result)
        {
            generateGoalStateEvent(goal.getLiteral(), goal.getType(), GoalStates.finished, result.ToString());
        }

        public void goalResumed(Trigger goal)
        {
            generateGoalStateEvent(goal.getLiteral(), goal.getType(), GoalStates.resumed, null);
        }

        public void goalStarted(Event goal)
        {
            generateGoalStateEvent(goal.getTrigger().getLiteral(), TEType.achieve, GoalStates.started, null);
        }

        public void goalSuspended(Trigger goal, string reason)
        {
            generateGoalStateEvent(goal.getLiteral(), goal.getType(), GoalStates.suspended, reason);
        }

        private void generateGoalStateEvent(Literal goal, TEType type, GoalStates state, string reason)
        {
            ts.runAtBeginOfNextCycle(new Runnable()
            {
                void run()
                {
                    Literal newGoal = goal.forceFullLiteralImpl().copy();
                    Literal stateAnnot = ASSyntax.createLiteral("state", new Atom(state.toString()));
                    if (reason != null)
                        stateAnnot.addAnnot(ASSyntax.createStructure("reason", new StringTermImpl(reason)));
                    newGoal.addAnnot(stateAnnot);
                    Trigger eEnd = new Trigger(TEOperator.goalState, type, newGoal);
                    if (ts.getAg().getPL().hasCandidatePlan(eEnd))
                        ts.getC().insertMetaEvent(new Event(eEnd, null));
                }
            });
        }
    }
}
