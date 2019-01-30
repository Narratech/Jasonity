using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jason.AsSemantics.Entities
{
    [Serializable]
    public class Circumstance
    {
        private static readonly long serialVersionUID = 1L;

        private Queue<Event> E;
        private Queue<Intention> I;
        protected ActionExec A;
        private Queue<Message> MB;
        protected List<Option> RP;
        protected List<Option> AP;
        protected Event SE;
        protected Option SO;
        protected Intention SI;
        private Intention AI; // Atomic Intention
        private Event AE; // Atomic Event
        private bool atomicIntSuspended = false; // whether the current atomic intention is suspended in PA or PI
                                                    //private   boolean                  hasAtomicEvent = false;

        private Dictionary<int, ActionExec> PA; // Pending actions, waiting action execution (key is the intention id)
        private List<ActionExec> FA; // Feedback actions, those that are already executed

        private Dictionary<string, Intention> PI; // pending intentions, intentions suspended by any other reason
        private Dictionary<string, Event> PE; // pending events, events suspended by .suspend

        private Queue<CircumstanceListener> listeners = new ConcurrentLinkedQueue<CircumstanceListener>();

        private TransitionSystem ts = null;

        public Object syncApPlanSense = new Object();

        public Circumstance()
        {
            create();
            reset();
        }

        public void setTS(TransitionSystem ts)
        {
            this.ts = ts;
        }

        /** creates new collections for E, I, MB, PA, PI, and FA */
        public void create()
        {
            // use LinkedList since we use a lot of remove(0) in selectEvent
            E = new ConcurrentLinkedQueue<Event>();
            I = new ConcurrentLinkedQueue<Intention>();
            MB = new ConcurrentLinkedQueue<Message>();
            PA = new ConcurrentHashMap<int, ActionExec>();
            PI = new ConcurrentHashMap<string, Intention>();
            PE = new ConcurrentHashMap<string, Event>();
            FA = new ArrayList<ActionExec>();
        }

        /** set null for A, RP, AP, SE, SO, and SI */
        public void reset()
        {
            resetSense();
            resetDeliberate();
            resetAct();
        }

        public void resetSense()
        {
        }

        public void resetDeliberate()
        {
            RP = null;
            AP = null;
            SE = null;
            SO = null;
        }

        public void resetAct()
        {
            A = null;
            SI = null;
        }

        public Event addAchvGoal(Literal l, Intention i)
        {
            Event evt = new Event(new Trigger(TEOperator.add, TEType.achieve, l), i);
            addEvent(evt);
            return evt;
        }

        public void addExternalEv(Trigger trig)
        {
            addEvent(new Event(trig, Intention.EmptyInt));
        }

        public void addExternalEv(Trigger trig)
        {
            addEvent(new Event(trig, Intention.EmptyInt));
        }

        /** Events */

        public void addEvent(Event ev)
        {

            if (ev.isAtomic())
                AE = ev;
            else
                E.Enqueue(ev);

            // notify listeners
            if (listeners != null)
            {
                foreach (CircumstanceListener el in listeners)
                    el.eventAdded(ev);
            }
        }

        public void insertMetaEvent(Event ev)
        {
            // meta events have to be placed in the begin of the queue, but not before other meta events
            List<Event> newE = new ArrayList<Event>(E); // make a list of events to find the best place to insert the new event
            int pos = 0;
            foreach (Event e in newE)
            {
                if (!e.getTrigger().isMetaEvent())
                {
                    break;
                }
                pos++;
            }
            newE.Add(pos, ev);
            E.Clear();
            foreach (Event e in newE)
            {
                E.Enqueue(e);
            }

            // notify listeners
            if (listeners != null)
            {
                foreach (CircumstanceListener el in listeners)
                    el.eventAdded(ev);
            }
        }

        public bool removeEvent(Event ev)
        {
            bool removed = false;
            if (ev.equals(AE))
            {
                AE = null;
                removed = true;
            }
            else
            {
                removed = E.Dequeue(ev);
            }
            if (removed && ev.getIntention() != null && listeners != null)
                foreach (CircumstanceListener el in listeners)
                    el.intentionDropped(ev.getIntention());
            return removed;
        }

        // remove events based on a match with a trigger
        public void removeEvents(Trigger te, Unifier un)
        {
            IEnumerator<Event> ie = E.GetEnumerator();
            while (ie.hasNext())
            {
                Event ev = ie.MoveNext();
                Trigger t = ev.getTrigger();
                if (ev.getIntention() != Intention.EmptyInt)
                { // since the unifier of the intention will not be used, apply it to the event before comparing to the event to be dropped
                    t = t.capply(ev.getIntention().peek().getUnif());
                }
                if (un.clone().unifiesNoUndo(te, t))
                {
                    ie.Reset(); //ie.remove()
                    if (ev.getIntention() != null && listeners != null)
                        foreach (CircumstanceListener el in listeners)
                            el.intentionDropped(ev.getIntention());
                }
            }
        }

        public void clearEvents()
        {
            // notify listeners
            if (listeners != null)
                foreach (CircumstanceListener el in listeners)
                {
                    foreach (Event ev in E)
                        if (ev.getIntention() != null)
                            el.intentionDropped(ev.getIntention());
                    if (AE != null && AE.getIntention() != null)
                        el.intentionDropped(AE.getIntention());
                }

            E.Clear();
            AE = null;
        }

        /** get the queue of events (which does not include the atomic event) */
        public Queue<Event> getEvents()
        {
            return E;
        }

        /** get the all events (which include the atomic event, if it exists) */
        public IEnumerator<Event> getEventsPlusAtomic()
        {
            if (AE == null)
            {
                return E.GetEnumerator();
            }
            else
            {
                List<Event> l = new ArrayList<Event>(E.size() + 1);
                l.add(AE);
                l.addAll(E);
                return l.iterator();
            }
        }
    }
}
