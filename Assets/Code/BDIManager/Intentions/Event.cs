// Interface for Events
// Allows the user to modify the events taking place in the environment
using Assets.Code.Logic;

namespace BDIManager.Intentions {
    public class Event
    {

        Trigger trigger = null;
        Intention intention = Intention.emptyInt;

        Event(Trigger t, Intention i)
        {
            trigger = t;
            intention = i;
        }

        Trigger GetTrigger()
        {
            return trigger;
        }

        Intention GetIntention()
        {
            return intention;
        }

        void SetIntention(Intention i)
        {
            intention = i;
        }

        bool SameTE(object t)
        {
            return trigger.Equals(t);
        }

        bool IsExternal()
        {
            return intention == Intention.emptyInt;
        }

        bool IsInternal()
        {
            return intention != Intention.emptyInt;
        }

        bool IsAtomic()
        {
            return intention != null && intention.IsAtomic();
        }

        int HashCode()
        {
            int r = trigger.GetHashCode();
            if (intention != null)
            {
                r = r + intention.GetHashCode();
            }
            return r;
        }
    }
}