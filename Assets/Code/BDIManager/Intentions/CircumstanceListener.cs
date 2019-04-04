using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDIManager.Intentions;

namespace Assets.Code.BDIManager
{
    interface CircumstanceListener 
    {
        void EventAdded(Event e);

        void IntentionAdded(Intention i);

        void IntentionDropped(Intention i);

        void IntentionResumed(Intention i);

        void IntentionSuspended(Intention i, string reason);
    }
}
