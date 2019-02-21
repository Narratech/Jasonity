using Assets.Code.Logic;
using BDIManager.Intentions;


/*
 Interface used to notificate changes in the states of the desires
*/
namespace BDIManager.Desires {
    public enum DesireStates { started, suspended, resumed, finished, failed };
    public enum FinishStates { achieved, unachieved, dropped };
    interface Desire {
        void DesireStarted(Event desire);
        void DesireFinished(Trigger desire, FinishStates result);
        void DesireFailed(Trigger desire);
        void DesireSuspended(Trigger desire, string reason);
        void DesireResumed(Trigger desire);
    }
}