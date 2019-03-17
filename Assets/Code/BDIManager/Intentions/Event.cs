// Interface for Events
// Allows the user to modify the events taking place in the environment
using Assets.Code.Logic;

namespace BDIManager.Intentions {
    class Event
    {

        private Trigger trigger = null;
        private Intention intention = Intention.emptyInt;

        Event(Trigger t, Intention i)
        {
            trigger = t;
            intention = i;
        }

        public Trigger GetTrigger()
        {
            return trigger;
        }

        public Intention GetIntention()
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