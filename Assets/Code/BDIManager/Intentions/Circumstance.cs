using Assets.Code.Agent;
using Assets.Code.AsSemantics;
using Assets.Code.AsSyntax;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;
using BDIMaAssets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;

namespace Assets.Code.BDIManager
{
    public class Circumstance
    {
        private Queue<ICircumstanceListener> listeners;
        private Queue<Event> E;
        private Queue<Intention> I;
        private ExecuteAction A;
        private Intention SI;

        internal void ClearEvents()
        {
            throw new NotImplementedException();
        }

        internal void ClearPendingEvents()
        {
            throw new NotImplementedException();
        }

        internal void RemoveEvents(Trigger te, Unifier un)
        {
            throw new NotImplementedException();
        }

        internal void RemovePendingEvents(Trigger te, Unifier un)
        {
            throw new NotImplementedException();
        }

        internal void ClearRunnigIntentions()
        {
            throw new NotImplementedException();
        }

        internal IEnumerator<Intention> GetAllIntentions()
        {
            throw new NotImplementedException();
        }

        internal void ClearPendingActions()
        {
            throw new NotImplementedException();
        }

        internal IEnumerator<Event> GetEventsPlusAtomic()
        {
            throw new NotImplementedException();
        }

        internal void RemoveEvent(Event e)
        {
            throw new NotImplementedException();
        }

        internal void DropIntention(Intention i)
        {
            throw new NotImplementedException();
        }

        internal object GetPendingEvents()
        {
            throw new NotImplementedException();
        }

        internal void ClearPendingIntentions()
        {
            throw new NotImplementedException();
        }

        internal void RemovePendingEvent(string ek)
        {
            throw new NotImplementedException();
        }

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

        public void AddEventListener(ICircumstanceListener cl)
        {
            listeners.Enqueue(cl);
        }

        public void RemoveEventListener(ICircumstanceListener cl)
        {
            if (cl != null)
            {
                listeners.Dequeue();
            }
        }

        public void ResetSense()
        {

        }

        public void AddExternalEv(Trigger trigger)
        {
            AddEvent(new Event(trigger, Intention.emptyInt));
        }

        public void AddEvent(Event ev)
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

        public bool HasMsg()
        {
            return MB.Count != 0;
        }

        public bool HasEvt()
        {
            return AE != null || E.Count != 0;
        }

        public IEnumerable<Event> GetEvents()
        {
            return E;
        }

        public void ResetDeliberate()
        {
            RP = null;
            AP = null;
            SE = null;
            SO = null;
        }

        public void ResetAct()
        {
            A = null;
            SI = null;
        }

        public ExecuteAction GetAction()
        {
            return A;
        }

        public void AddPendingAction(ExecuteAction action)
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

        internal void InsertMetaEvent(Event @event)
        {
            throw new NotImplementedException();
        }

        private void SetAtomicIntention(Intention i)
        {
            AI = i;
        }

        public void SetReasoner(Reasoner reasoner)
        {
            this.reasoner = reasoner;
        }

        public bool DropRunningIntention(Intention i)
        {
            if (RemoveRunningIntention(i))
            {
                if (listeners != null)
                {
                    foreach (var el in listeners)
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

        internal void AddFeedbackAction(ExecuteAction action) // SYNCHRONIZED
        {
            if (action.GetIntention() != null)
            {
                // synchronized (FA) {
                    FA.Add(action);
                // }
            }
        }

        // Creates new collections for E, I, MB, PA, PI, and FA
        public void Create()
        {
            E = new Queue<Event>();
            I = new Queue<Intention>();
            MB = new Queue<Message>();
            PA = new Dictionary<int, ExecuteAction>();
            PI = new Dictionary<string, Intention>();
            PE = new Dictionary<string, Event>();
            FA = new List<ExecuteAction>();
        }

        public bool HasRunningIntention()
        {
            return (I != null && I.Count != 0) || AI != null;
        }

        public bool HasEvent()
        {
            return AE != null || E.Count != 0;
        }

        public bool IsAtomicIntentionSuspended()
        {
            return AI != null && atomicIntSuspended;
        }

        public bool HasFeedbackAction()
        {
            return FA.Count != 0;
        }

        public object GetMailBox()
        {
            return MB;
        }

        public object GetPendingIntentions()
        {
            return PI;
        }

        public Intention RemovePendingIntention(string msgId)
        {
            Intention i = PI[msgId];
            PI.Remove(msgId);
            if (i != null && i.IsAtomic())
            {
                atomicIntSuspended = false;
            }
            return i;
        }

        public void ResumeIntention(Intention i)
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

        internal byte GetNbRunningIntentions()
        {
            throw new NotImplementedException();
        }

        public void AddRunningIntention(Intention i)
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

        public bool HasAtomicIntention()
        {
            return AI != null;
        }

        // Remove and returns the event with atomic intention, null if none
        public Event RemoveAtomicEvent()
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

        internal object GetSelectedOption()
        {
            throw new NotImplementedException();
        }

        public void SetSE(Event ev)
        {
            SE = ev;
        }

        public void SetRP(List<Option> rp)
        {
            RP = rp;
        }

        public List<Option> GetRp()
        {
            return RP;
        }

        internal void SetSO(Option o)
        {
            SO = o;
        }

        internal object GetAP()
        {
            return AP;
        }

        public Option GetSO()
        {
            return SO;
        }

        internal Event AddAchieveDesire(Literal body, Intention curInt)
        {
            Event evt = new Event(new Trigger(TEOperator.add, TEType.achieve, body), curInt);
            AddEvent(evt);
            return evt;
        }

        internal Circumstance Clone()
        {
            throw new NotImplementedException();
        }

        internal bool HasListener()
        {
            return listeners.Count != 0;
        }

        internal void AddAchvGoal(Literal d, Intention emptyInt)
        {
            throw new NotImplementedException();
        }

        internal void AddMsg(Message im)
        {
            throw new NotImplementedException();
        }

        internal void SetA(ExecuteAction executeAction)
        {
            throw new NotImplementedException();
        }

        internal Circumstance GetFirst()
        {
            throw new NotImplementedException();
        }

        internal void SetSI(object p)
        {
            throw new NotImplementedException();
        }

        internal object RemoveAtomicIntention()
        {
            throw new NotImplementedException();
        }

        internal Queue<Intention> GetRunningIntentions()
        {
            throw new NotImplementedException();
        }

        internal void SetAP(List<Option> list)
        {
            throw new NotImplementedException();
        }

        internal object GetSelectedEvent()
        {
            throw new NotImplementedException();
        }

        internal object GetSelectedIntention()
        {
            throw new NotImplementedException();
        }
    }
}