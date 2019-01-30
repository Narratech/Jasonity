using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jason.AsSemantics.AsSemantics
{
    public enum GoalStates { started, suspended, resumed, finished, failed };
    public enum FinishStates { achieved, unachieved, dropped };

    public interface GoalListener
    {
        /** method called when a new goal is produced by operator ! */
        void goalStarted(Event goal);

        /** method called when a goal is (un)successfully finished */
        void goalFinished(Trigger goal, FinishStates result);

        /** method called when a goal is failed */
        void goalFailed(Trigger goal);

        /** method called when a goal is suspended (waiting action on the environment or due to internal actions like .wait and .suspend) */
        void goalSuspended(Trigger goal, string reason);

        /** called when a suspended goal is resumed */
        void goalResumed(Trigger goal);
    }
}
