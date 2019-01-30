using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jason.AsSemantics.AsSemantics
{
    [Serializable]
    public class Event
    {
        private static readonly long serialVersionUID = 1L;

        Trigger trigger = null;
        Intention intention = Intention.EmptyInt;

        public Event(Trigger t, Intention i)
        {
            trigger = t;
            intention = i;
        }

        public Trigger getTrigger()
        {
            return trigger;
        }

        public Intention getIntention()
        {
            return intention;
        }
        public void setIntention(Intention i)
        {
            intention = i;
        }

        public bool sameTE(Object t)
        {
            return trigger.equals(t);
        }

        public bool isExternal()
        {
            return intention == Intention.EmptyInt;
        }
        public bool isInternal()
        {
            return intention != Intention.EmptyInt;
        }
        public bool isAtomic()
        {
            return intention != null && intention.isAtomic();
        }

        public bool equals(Object o)
        {
            if (o == null) return false;
            if (o == this) return true;
            if (o.GetType().IsInstanceOfType(Event)) {
                Event oe = (Event)o;
                if (this.intention == null && oe.intention != null) return false;
                if (this.intention != null && !this.intention.equals(oe.intention)) return false;

                return this.trigger.equals(oe.trigger);
            }
            return false;
        }

        public Object clone()
        {
            Trigger tc = (trigger == null ? null : (Trigger)trigger.clone());
            Intention ic = (intention == null ? null : (Intention)intention.clone());
            return new Event(tc, ic);
        }

        public string toString()
        {
            if (intention == Intention.EmptyInt)
                return "" + trigger;
            else
                return trigger + "\n" + intention;
        }

        /** get as XML */
        //public Element getAsDOM(Document document)
        //{
        //    Element eevt = (Element)document.createElement("event");
        //    eevt.appendChild(trigger.getAsDOM(document));
        //    if (intention != Intention.EmptyInt)
        //    {
        //        eevt.setAttribute("intention", intention.getId() + "");
        //    }
        //    return eevt;
        //}
    }
}
