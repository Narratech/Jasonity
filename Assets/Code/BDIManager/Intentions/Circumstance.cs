using Assets.Code.Logic;
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
    }
}
