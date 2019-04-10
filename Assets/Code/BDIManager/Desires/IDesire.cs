/*
 Interface used to notify changes in the states of the desires
*/
using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;

namespace BDIManager.Desires {
    public enum DesireStates { started, suspended, resumed, finished, failed };
    public enum FinishStates { achieved, unachieved, dropped };

    interface IDesire {
        // New desire produced by operator
        void DesireStarted(Event desire);

        // Desire (un)successfully finished
        void DesireFinished(Trigger desire, FinishStates result);

        // Desire failed
        void DesireFailed(Trigger desire);

        // Desire is suspended (because of environment or internal actions)
        void DesireSuspended(Trigger desire, string reason);

        // Desire is resumed
        void DesireResumed(Trigger desire);
        object AllDesires(Circumstance circumstance, Literal body, object p, Unifier unifier);
    }
}