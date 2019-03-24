using Assets.Code.Agent;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;
using BDIMaAssets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.BDIManager
{
    class Circumstance
    {
        private Queue<CircumstanceListener> listeners;
        private Queue<Event> E;
        private Queue<Intention> I;
        private ExecuteAction A;
        private Intention SI;
        private Event SE;
        private Option SO;
        private Event AE; // Atomic Event
        private Queue<Message> MB;
        private List<Option> RP;
        private List<Option> AP;
        private bool atomicIntSuspended = false;
        private Dictionary<int, ExecuteAction> PA; // Pending actions, waiting execution
        private Intention AI;
        private Reasoner reasoner = null;
        private Dictionary<string, Intention> PI; // Pending intentions, suspended
        private Dictionary<string, Event> PE; // Pending events, suspended
        private List<ExecuteAction> FA; // Feedback actions, already executed

        public Intention GetSI()
        {
            return SI;
        }

        internal void AddEventListener(CircumstanceListener cl)
        {
            listeners.Enqueue(cl);
        }

        internal void RemoveEventListener(CircumstanceListener cl)
        {
            if (cl != null)
            {
                listeners.Dequeue(); // ???
            }
        }

        public void ResetSense()
        {

        }

        internal void AddExternalEv(Trigger trigger)
        {
            AddEvent(new Event(trigger, Intention.emptyInt));
        }

        private void AddEvent(Event ev)
        {
            if (ev.IsAtomic())
            {
                AE = ev;
            }
            else
            {
                E.Enqueue(ev);
            }

            // Notify listeners
            if (listeners != null)
            {
                foreach (var el in listeners)
                {
                    el.EventAdded(ev);
                }
            }
        }

        internal bool HasMsg()
        {
            return MB.Count != 0;
        }

        internal bool HasEvt()
        {
            return AE != null || E.Count != 0;
        }

        internal IEnumerable<Event> GetEvents()
        {
            return E;
        }

        internal void ResetDeliberate()
        {
            RP = null;
            AP = null;
            SE = null;
            SO = null;
        }

        internal void ResetAct()
        {
            A = null;
            SI = null;
        }

        internal ExecuteAction GetAction()
        {
            return A;
        }

        internal void AddPendingAction(ExecuteAction action)
        {
            Intention i = action.GetIntention();
            if (i.IsAtomic())
            {
                SetAtomicIntention(i);
                atomicIntSuspended = true;
            }

            PA.Add(i.GetID(), action);
            i.SetSuspendedReason(action.GetActionTerm().ToString());

            if (listeners != null)
            {
                foreach (var el in listeners)
                {
                    el.IntentionSuspended(i, "action " + action.GetActionTerm());
                }
            }
        }

        private void SetAtomicIntention(Intention i)
        {
            AI = i;
        }

        internal void SetReasoner(Reasoner reasoner)
        {
            this.reasoner = reasoner;
        }

        internal bool DropRunningIntention(Intention i)
        {
            if (RemoveRunningIntention(i))
            {
                if (listeners != null)
                {
                    for (CircumstanceListener el : listeners)
                    {
                        el.IntentionDropped(i);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool RemoveRunningIntention(Intention i)
        {
            if (i == AI)
            {
                SetAtomicIntention(null);
                return true;
            }
            else
            {
                return I.Remove(i); // ???
            }
        }

        // Creates new collections for E, I, MB, PA, PI, and FA
        internal void Create()
        {
            E = new Queue<Event>();
            I = new Queue<Intention>();
            MB = new Queue<Message>();
            PA = new Dictionary<int, ExecuteAction>();
            PI = new Dictionary<string, Intention>();
            PE = new Dictionary<string, Event>();
            FA = new List<ExecuteAction>();
        }

        internal bool HasRunningIntention()
        {
            return (I != null && I.Count != 0) || AI != null;
        }

        internal bool HasEvent()
        {
            return AE != null || E.Count != 0;
        }

        internal bool IsAtomicIntentionSuspended()
        {
            return AI != null && atomicIntSuspended;
        }

        internal bool HasFeedbackAction()
        {
            return FA.Count != 0;
        }

        internal object GetMailBox()
        {
            return MB;
        }

        internal object GetPendingIntentions()
        {
            return PI;
        }

        internal Intention RemovePendingIntention(string msgId)
        {
            Intention i = PI.Remove(msgId); // ???
            if (i != null && i.IsAtomic())
            {
                atomicIntSuspended = false;
            }
            return i;
        }

        internal void ResumeIntention(Intention i)
        {
            AddRunningIntention(i);

            // Notify meta event listeners
            if (listeners != null)
            {
                foreach (var el in listeners) // ???
                {
                    el.IntentionResumed(i);
                }
            }
        }

        private void AddRunningIntention(Intention i)
        {
            if (i.IsAtomic())
            {
                SetAtomicIntention(i);
            }
            else
            {
                I.Enqueue(i);
            }
        }

        public Event GetSE()
        {
            return SE;
        }

        internal bool HasAtomicIntention()
        {
            return AI != null;
        }

        // Remove and returns the event with atomic intention, null if none
        internal Event RemoveAtomicEvent()
        {
            Event e = AE;
            AE = null;
            if (e != null && e.GetIntention() != null && listeners != null)
            {
                foreach (var el in listeners)
                {
                    el.IntentionDropped(e.GetIntention());
                }
            }
            return e;
        }

        internal void SetSE(Event ev)
        {
            SE = ev;
        }

        internal void SetRP(List<Option> rp)
        {
            RP = rp;
        }

        internal List<Option> GetRp()
        {
            return RP;
        }
    }
}