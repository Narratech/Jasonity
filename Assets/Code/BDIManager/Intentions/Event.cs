// Interface for Events
// Allows the user to modify the events taking place in the environment
using Assets.Code.Logic;

namespace BDIManager.Intentions {
    public interface Event
    {
        Trigger GetTrigger();
    }
}