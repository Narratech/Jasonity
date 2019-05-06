using Assets.Code.AsSemantics;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using BDIMaAssets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Assets.Code.BDIManager
{
    public class Circumstance
    {
        private List<Event> E;
        private List<Intention> I;
        private ExecuteAction A;
        private Queue<Message> MB;
        private List<Option> RP;
        private List<Option> AP;
        private Event SE;
        private Option SO;
        private Intention SI;
        private Intention AI;
        private Event AE; // Atomic Event
        private bool atomicIntSuspended = false;

        private Dictionary<int, ExecuteAction> PA; // Pending actions, waiting execution
        private List<ExecuteAction> FA; // Feedback actions, already executed

        private Dictionary<string, Intention> PI; // Pending intentions, suspended
        private Dictionary<string, Event> PE; // Pending events, suspended

        

        private Queue<ICircumstanceListener> listeners;

        private Reasoner reasoner = null;

        public object syncApPlanSense = new object();

        public Circumstance()
        {
            Create();
            Reset();
        }

        public void SetReasoner(Reasoner reasoner) => this.reasoner = reasoner;

        // Creates new collections for E, I, MB, PA, PI, and FA
        public void Create()
        {
            E = new List<Event>();
            I = new List<Intention>();
            MB = new Queue<Message>();
            PA = new Dictionary<int, ExecuteAction>();
            PI = new Dictionary<string, Intention>();
            PE = new Dictionary<string, Event>();
            FA = new List<ExecuteAction>();
        }

        public void Reset()
        {
            ResetSense();
            ResetDeliberate();
            ResetAct();
        }

        public void ResetSense() { } // Was empty in original

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

        internal Event AddAchieveDesire(Literal body, Intention curInt)
        {
            Event evt = new Event(new Trigger(TEOperator.add, TEType.achieve, body), curInt);
            AddEvent(evt);
            return evt;
        }

        public void AddExternalEv(Trigger trigger)
        {
            AddEvent(new Event(trigger, Intention.emptyInt));
        }

        /* Events */

        public void AddEvent(Event ev)
        {
            if (ev.IsAtomic()) AE = ev;
            else E.Add(ev);

            // Notify listeners
            if (listeners != null)
            {
                foreach (var el in listeners) el.EventAdded(ev);
            }
        }

        public void InsertMetaEvent(Event ev)
        {
            // Meta events have to be placed in the beginning of the queue, but not before other meta events
            List<Event> newE = new List<Event>(E);
            int pos = 0;
            foreach (Event e in newE)
            {
                if (!e.GetTrigger().IsMetaEvent()) break;
                pos++;
            }
            newE.Insert(pos, ev);
            E.Clear();
            foreach (Event e in newE)
            {
                E.Add(e);
            }
            //E.AddRange(newE);

            // Notify listeners
            if (listeners != null)
                foreach (ICircumstanceListener el in listeners) el.EventAdded(ev);
        }

        public bool RemoveEvent(Event ev)
        {
            bool removed = false;
            if (ev.Equals(AE))
            {
                AE = null;
                removed = true;
            }
            else removed = E.Remove(ev); // ?????
            if (removed && ev.GetIntention() != null && listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    el.IntentionDropped(ev.GetIntention());
            return removed;
        }

        public void RemoveEvents(Trigger te, Unifier un)
        {
            IEnumerator<Event> ie = E.GetEnumerator();
            while (ie.MoveNext())
            {
                Event ev = ie.Current;
                Trigger t = ev.GetTrigger();
                if (ev.GetIntention() != Intention.emptyInt)
                    t = t.Capply(ev.GetIntention().Peek().GetUnif());
                if (un.Clone().UnifiesNoUndo(te, t))
                {
                    ie.Dispose();
                    if (ev.GetIntention() != null && listeners != null)
                        foreach (ICircumstanceListener el in listeners)
                            el.IntentionDropped(ev.GetIntention());
                }
            }
        }

        public void ClearEvents()
        {
            // Notify listeners
            if (listeners != null)
                foreach (ICircumstanceListener el in listeners)
                {
                    foreach (Event ev in E)
                        if (ev.GetIntention() != null)
                            el.IntentionDropped(ev.GetIntention());
                    if (AE != null && AE.GetIntention() != null)
                        el.IntentionDropped(AE.GetIntention());
                }
            E.Clear();
            AE = null;
        }

        public IEnumerable<Event> GetEvents() => E;

        public IEnumerator<Event> GetEventsPlusAtomic()
        {
            if (AE == null) return E.GetEnumerator();
            else
            {
                List<Event> l = new List<Event>(E.Count + 1);
                l.Add(AE);
                l.AddRange(E);
                return l.GetEnumerator();
            }
        }

        public bool HasEvent() => AE != null || E.Count != 0;

        public Event GetAtomicEvent() => AE;

        public Event RemoveAtomicEvent()
        {
            Event e = AE;
            AE = null;
            if (e != null && e.GetIntention() != null && listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    el.IntentionDropped(e.GetIntention());
            return e;
        }

        /* Listeners */

        public void AddEventListener(ICircumstanceListener cl) => listeners.Enqueue(cl);

        public void RemoveEventListener(ICircumstanceListener cl)
        {
            if (cl != null) listeners.Dequeue();
        }

        public bool HasListener() => listeners.Count != 0;

        public Queue<ICircumstanceListener> GetListeners() => listeners;

        /* Messages */

        public Queue<Message> GetMailBox() => MB;

        public void AddMsg(Message m) => MB.Enqueue(m);

        public bool HasMsg() => MB.Count != 0;

        /* Intentions */

        public List<Intention> GetRunningIntentions() => I;

        public IEnumerator<Intention> GetRunningIntentionsPlusAtomic()
        {
            if (AI == null) return I.GetEnumerator();
            else
            {
                List<Intention> l = new List<Intention>(I.Count + 1);
                l.Add(AI);
                l.AddRange(I);
                return l.GetEnumerator();
            }
        }

        public int GetNbRunningIntentions()
        {
            int n = I.Count;
            if (AI != null) n++;
            if (SI != null && SI != AI) n++;
            return n;
        }

        public bool HasRunningIntention() => (I != null && I.Count != 0) || AI != null;
        public bool HasRunningIntention(Intention i) => i == AI || I.Contains(i);

        public void AddRunningIntention(Intention i)
        {
            if (i.IsAtomic()) SetAtomicIntention(i);
            else I.Add(i);

            // Notify
            if (listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    el.IntentionAdded(i);
        }

        // Add the intention back to I, and also notify meta listeners that desires are resumed
        public void ResumeIntention(Intention intention)
        {
            AddRunningIntention(intention);

            // Notify meta event listeners
            if (listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    el.IntentionResumed(intention);
        }
        
        // Remove intention from I
        public bool RemoveRunningIntention(Intention i)
        {
            if (i == AI)
            {
                SetAtomicIntention(null);
                return true;
            }
            else return I.Remove(i);
        }

        // Removes and produces events to signal the intention was dropped
        public bool DropRunningIntention(Intention i)
        {
            if (RemoveRunningIntention(i))
            {
                if (listeners != null)
                    foreach (ICircumstanceListener el in listeners)
                        el.IntentionDropped(i);
                return true;
            }
            else return false;
        }

        public void ClearRunningIntentions()
        {
            SetAtomicIntention(null);

            if (listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    foreach (Intention i in I)
                        el.IntentionDropped(i);
            I.Clear();
        }

        public void SetAtomicIntention(Intention i) => AI = i;

        public Intention RemoveAtomicIntention()
        {
            if (AI != null)
            {
                if (atomicIntSuspended) return null;
                Intention tmp = AI;
                RemoveRunningIntention(AI);
                return tmp;
            }
            return null;
        }

        public bool HasAtomicIntention() => AI != null;

        public bool IsAtomicIntentionSuspended() => AI != null && atomicIntSuspended;

        /* Atomic intentions */

        public Dictionary<string, Intention> GetPendingIntentions() => PI;

        public bool HasPendingIntention() => PI != null && PI.Count != 0;

        public void ClearPendingIntentions()
        {
            // Notify listeners
            if (listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    foreach (Intention i in PI.Values)
                        el.IntentionDropped(i);
            PI.Clear();
        }

        public void AddPendingIntention(string id, Intention i)
        {
            if (i.IsAtomic())
            {
                SetAtomicIntention(i);
                atomicIntSuspended = true;
            }
            PI.Add(id, i);
            i.SetSuspendedReason(id);

            if (listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    el.IntentionSuspended(i, id);
        }

        public Intention RemovePendingIntention(string pendingID)
        {
            Intention i = PI[pendingID];
            PI.Remove(pendingID);
            if (i != null && i.IsAtomic()) atomicIntSuspended = false;
            return i;
        }
        public Intention RemovePendingIntention(int intentionID)
        {
            foreach (string key in PI.Keys)
            {
                Intention pii = PI[key];
                if (pii.GetID() == intentionID) return RemovePendingIntention(key);
            }
            return null;
        }

        // Removes intention "i" from PI and notifies listeners the intention was dropped
        public bool DropPendingIntention(Intention i)
        {
            foreach (string key in PI.Keys)
            {
                Intention pii = PI[key];
                if (pii.Equals(i))
                {
                    RemovePendingIntention(key);

                    // Check in wait internal action
                    if (listeners != null)
                        foreach (ICircumstanceListener el in listeners)
                            el.IntentionDropped(i);
                    return true;
                }
            }
            return false;
        }

        /* Pending events */
        public Dictionary<string, Event> GetPendingEvents() => PE;

        public bool HasPendingEvent() => PE != null && PE.Count > 0;

        public void ClearPendingEvents()
        {
            // Notify listeners
            if (listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    foreach (Event e in PE.Values)
                        if (e.GetIntention() != null)
                            el.IntentionDropped(e.GetIntention());
            PE.Clear();
        }

        public void AddPendingEvent(string id, Event e)
        {
            PE.Add(id, e);

            if (listeners != null && e.GetIntention() != null)
                foreach (ICircumstanceListener el in listeners)
                    el.IntentionSuspended(e.GetIntention(), id);
        }

        public Event RemovePendingEvent(string pendingID)
        {
            Event e = PE[pendingID];
            PE.Remove(pendingID);
            if (e != null && listeners != null && e.GetIntention() != null)
                foreach (ICircumstanceListener el in listeners)
                    el.IntentionDropped(e.GetIntention());
            return e;
        }

        public void RemovePendingEvents(Trigger te, Unifier un)
        {
            IEnumerator<Event> ie = PE.Values.GetEnumerator();
            while (ie.MoveNext())
            {
                Event ev = ie.Current;
                Trigger t = ev.GetTrigger();
                if (ev.GetIntention() != Intention.emptyInt)
                    t = t.Capply(ev.GetIntention().Peek().GetUnif());
                if (un.Clone().UnifiesNoUndo(te, t))
                {
                    ie.Dispose();

                    if (listeners != null && ev.GetIntention() != null)
                        foreach (ICircumstanceListener el in listeners)
                            el.IntentionDropped(ev.GetIntention());
                }
            }
        }

        /* Actions */

        public ExecuteAction GetAction() => A;

        public void SetAction(ExecuteAction a) => A = a;

        public List<Option> GetApplicablePlans() => AP;

        /* Feedback action */

        public bool HasFeedbackAction() => FA.Count != 0;

        public List<ExecuteAction> GetFeedbackActions() => FA;

        public void AddFeedbackAction(ExecuteAction act)
        {
            if (act.GetIntention() != null) /* synchronized (FA) */ FA.Add(act);
        }

        /* Pending action */

        public Dictionary<int, ExecuteAction> GetPendingActions() => PA;

        public void AddPendingAction(ExecuteAction a)
        {
            Intention i = a.GetIntention();
            if (i.IsAtomic())
            {
                SetAtomicIntention(i);
                atomicIntSuspended = true;
            }
            PA.Add(i.GetID(), a);
            i.SetSuspendedReason(a.GetActionTerm().ToString());

            if (listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    el.IntentionSuspended(i, "action " + a.GetActionTerm());
        }

        public void ClearPendingActions()
        {
            // Notify listeners
            if (listeners != null)
                foreach (ICircumstanceListener el in listeners)
                    foreach (ExecuteAction act in PA.Values)
                        el.IntentionDropped(act.GetIntention());
            PA.Clear();
        }

        public bool HasPendingAction() => PA != null && PA.Count != 0;

        public ExecuteAction RemovePendingAction(int intentionID)
        {
            ExecuteAction a = PA[intentionID];
            PA.Remove(intentionID);
            if (a != null && a.GetIntention().IsAtomic()) atomicIntSuspended = false;
            return a;
        }

        // Removes intention from PA and notifies listeners the intention was dropped
        public bool DropPendingAction(Intention i)
        {
            ExecuteAction act = RemovePendingAction(i.GetID());
            if (act != null)
            {
                if (listeners != null)
                    foreach (ICircumstanceListener el in listeners)
                        el.IntentionDropped(i);
                return true;
            }
            return false;
        }
       
        public void DropIntention(Intention del)
        {
            // Intention may be suspended in E or PE
            IEnumerator<Event> ie = GetEventsPlusAtomic();
            while (ie.MoveNext())
            {
                Event e = ie.Current;
                Intention i = e.GetIntention();
                if (i != null && i.Equals(del)) RemoveEvent(e);
            }
            foreach (string k in GetPendingEvents().Keys)
            {
                Intention i = GetPendingEvents()[k].GetIntention();
                if (i != null && i.Equals(del)) RemovePendingEvent(k);
            }

            // Intention may be suspended in PA
            DropPendingAction(del);
            DropRunningIntention(del);

            // Intention may be suspended in PI
            DropPendingIntention(del);
        }

        public List<Option> GetRelevantPlans() => RP;

        public Event GetSelectedEvent() => SE;

        public Intention GetSelectedIntention() => SI;

        public Option GetSelectedOption() => SO;

        // Clone E, I, MB, PA, PI, FA, and AI
        public Circumstance Clone()
        {
            Circumstance c = new Circumstance();
            if (AE != null)
                c.AE = (Event)AE.Clone();
            c.atomicIntSuspended = atomicIntSuspended;

            foreach (Event e in E)
                c.E.Add(e.Clone());
            foreach (Intention i in I)
                c.I.Add(i.Clone());
            foreach (Message m in MB)
                c.MB.Enqueue(m.Clone());
            foreach (int k in PA.Keys)
                c.PA.Add(k, PA[k].Clone());
            foreach (string k in PI.Keys)
                c.PI.Add(k, PI[k].Clone());
            foreach (string k in PE.Keys)
                c.PE.Add(k, PE[k].Clone());
            foreach (ExecuteAction ae in FA)
                c.FA.Add(ae.Clone());
            return c;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("Circumstance:\n");
            s.Append("  E =" + E + "\n");
            s.Append("  I =" + I + "\n");
            s.Append("  A =" + A + "\n");
            s.Append("  MB=" + MB + "\n");
            s.Append("  RP=" + RP + "\n");
            s.Append("  AP=" + AP + "\n");
            s.Append("  SE=" + SE + "\n");
            s.Append("  SO=" + SO + "\n");
            s.Append("  SI=" + SI + "\n");
            s.Append("  AI=" + AI + "\n");
            s.Append("  AE=" + AE + "\n");
            s.Append("  PA=" + PA + "\n");
            s.Append("  PI=" + PI + "\n");
            s.Append("  FA=" + FA + "\n");
            return s.ToString();
        }

        public IEnumerator<Intention> GetAllIntentions()
        {
            return new EnumeratorIntentions<Intention>(this);
        }

        private class EnumeratorIntentions<T> : IEnumerator<Intention>
        {
            // Data structure with intentions
            enum Step { selEvt, selInt, evt, pendEvt, pendAct, pendInt, intentions, end }
            Circumstance c; 
            Step curStep = Step.selEvt;
            Intention curInt = null;
            IEnumerator<Event> evtEnumerator = null;
            IEnumerator<Event> pendEvtEnumerator = null;
            IEnumerator<ExecuteAction> pendActEnumerator = null;
            IEnumerator<Intention> pendIntEnumerator = null;
            IEnumerator<Intention> intEnumerator = null;

            public EnumeratorIntentions(Circumstance c)
            {
                this.c = c;
                //Ponemos el método en el constructor, fuera del bloque estático
                Find();
            }

            public Intention Current => intEnumerator.Current;

            object IEnumerator.Current => intEnumerator.Current;

            //static EnumeratorIntentions(){ Find(); }

            public bool HasNext() => curInt != null;

            public Intention Next()
            {
                if (curInt == null) Find();
                Intention b = curInt;
                Find();
                return b;
            }
            public void Remove() { }

            void Find()
            {
                switch (curStep)
                {
                    case Step.selEvt:
                        curStep = Step.selInt; // Set next step
                                               // We must check the intention in the selected event in this cycle
                        if (c.GetSelectedEvent() != null)
                        {
                            curInt = c.GetSelectedEvent().GetIntention();
                            if (curInt != null) return;
                        }
                        Find();
                        return;

                    case Step.selInt:
                        curStep = Step.evt; // Set next step
                                            // We need to check the selected intention in this cycle
                        Intention prev = curInt;
                        curInt = c.GetSelectedIntention();
                        if (curInt != null && !curInt.Equals(prev)) return;
                        Find();
                        return;

                    case Step.evt:
                        if (evtEnumerator == null) evtEnumerator = c.GetEventsPlusAtomic();
                        while (evtEnumerator.MoveNext())
                        {
                            curInt = evtEnumerator.Current.GetIntention();
                            if (curInt != null && !curInt.Equals(c.GetSelectedIntention())) return;
                        }
                        curStep = Step.pendEvt; // set next step
                        Find();
                        return;

                    case Step.pendEvt:
                        if (pendEvtEnumerator == null)
                            pendEvtEnumerator = c.GetPendingEvents().Values.GetEnumerator();
                        while (pendEvtEnumerator.MoveNext())
                        {
                            curInt = pendEvtEnumerator.Current.GetIntention();
                            if (curInt != null) return;
                        }
                        curStep = Step.pendAct; // set next step
                        Find();
                        return;

                    case Step.pendAct:
                        // Intention may be suspended in PA
                        if (c.HasPendingAction())
                        {
                            if (pendActEnumerator == null)
                                pendActEnumerator = c.GetPendingActions().Values.GetEnumerator();
                            while (pendActEnumerator.MoveNext())
                            {
                                curInt = pendActEnumerator.Current.GetIntention();
                                if (curInt != null) return;
                            }
                        }
                        curStep = Step.pendInt; // next step
                        Find();
                        return;

                    case Step.pendInt:
                        if (c.HasPendingIntention())
                        {
                            if (pendIntEnumerator == null)
                                pendIntEnumerator = c.GetPendingIntentions().Values.GetEnumerator();

                            if (pendIntEnumerator.MoveNext())
                            {
                                curInt = pendIntEnumerator.Current;
                                return;
                            }
                        }
                        curStep = Step.intentions; // next step
                        Find();
                        return;

                    case Step.intentions:
                        if (intEnumerator == null)
                            intEnumerator = c.GetRunningIntentionsPlusAtomic();

                        if (intEnumerator.MoveNext())
                        {
                            curInt = intEnumerator.Current;
                            return;
                        }

                        curStep = Step.end; // next step
                        Find();
                        return;

                    case Step.end:
                        break;

                }
                curInt = null; // nothing found
            }

            public bool MoveNext()
            {
                return intEnumerator.MoveNext();
            }

            public void Reset()
            {
                intEnumerator.Reset();
            }

            public void Dispose()
            {
                intEnumerator.Dispose();
            }
        }

        public void SetSelectedIntention(Intention intention)
        {
            SI = intention;
        }

        public void SetSelectedOption(Option option)
        {
            SO = option;
        }

        public void SetRelevantPlans(List<Option> list)
        {
            foreach (Option l in list)
            {
                RP.Add(l);
            }
        }

        public void SetApplicablePlans(List<Option> list)
        {
            foreach (Option l in list)
            {
                AP.Add(l);
            }
        }

        public void SetSelectedEvent(Event @event)
        {
            SE = @event;
        }
    }
}