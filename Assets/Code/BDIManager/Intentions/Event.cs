// Interface for Events
// Allows the user to modify the events taking place in the environment
using Assets.Code.AsSyntax;
using Assets.Code.Logic;

namespace BDIManager.Intentions {
    class Event
    {
        private Trigger trigger = null;
        private Intention intention = Intention.emptyInt;

        public Event(Trigger t, Intention i)
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

        public void SetIntention(Intention i)
        {
            intention = i;
        }

        public bool SameTE(object t)
        {
            return trigger.Equals(t);
        }

        public bool IsExternal()
        {
            return intention == Intention.emptyInt;
        }

        public bool IsInternal()
        {
            return intention != Intention.emptyInt;
        }

        public bool IsAtomic()
        {
            return intention != null && intention.IsAtomic();
        }

        public int HashCode()
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