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
        private Intention SI;



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

        private void AddEvent(Event @event)
        {
            throw new NotImplementedException();
        }

        internal bool HashMsg()
        {
            throw new NotImplementedException();
        }

        internal bool HasEvt()
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<Event> GetEvents()
        {
            throw new NotImplementedException();
        }

        internal void ResetDeliberate()
        {
            throw new NotImplementedException();
        }

        internal void ResetAct()
        {
            throw new NotImplementedException();
        }

        internal ExecuteAction GetAction()
        {
            throw new NotImplementedException();
        }

        internal void AddPendingAction(ExecuteAction action)
        {
            throw new NotImplementedException();
        }

        internal void SetReasoner(Reasoner reasoner)
        {
            throw new NotImplementedException();
        }

        internal void DropRunningIntention(Intention i)
        {
            throw new NotImplementedException();
        }

        internal void Create()
        {
            throw new NotImplementedException();
        }

        internal bool HasMsg()
        {
            throw new NotImplementedException();
        }

        internal bool HasRunningIntention()
        {
            throw new NotImplementedException();
        }

        internal bool HasEvent()
        {
            throw new NotImplementedException();
        }

        internal bool IsAtomicIntentionSuspended()
        {
            throw new NotImplementedException();
        }

        internal bool HasFeedbackAction()
        {
            throw new NotImplementedException();
        }
    }
}
