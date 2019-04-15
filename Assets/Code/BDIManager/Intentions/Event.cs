// Interface for Events
// Allows the user to modify the events taking place in the environment
using Assets.Code.AsSyntax;
using Assets.Code.Logic;

namespace BDIManager.Intentions {
    public class Event
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

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o == this) return true;
            if (o is Event)
            {
                Event oe = (Event)o;
                if (intention == null && oe.intention != null) return false;
                if (intention != null && !intention.Equals(oe.intention)) return false;

                return trigger.Equals(oe.trigger);
            }
            return false;
        }

        public object Clone()
        {
            Trigger tc = (trigger == null ? null : trigger.Clone());
            Intention ic = (intention == null ? null : intention.Clone());
            return new Event(tc, ic);
        }

        public override string ToString()
        {
            if (intention == Intention.emptyInt) return "" + trigger;
            else return trigger + "\n" + intention;
        }
    }
}