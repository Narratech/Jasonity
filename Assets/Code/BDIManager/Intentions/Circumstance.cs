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
    }
}
