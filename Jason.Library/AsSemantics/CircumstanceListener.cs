using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jason.AsSemantics.Entities
{
    interface CircumstanceListener
    {
        void eventAdded(Event e);
        void intentionAdded(Intention i);
        void intentionDropped(Intention i);
        void intentionSuspended(Intention i, String reason);
        void intentionResumed(Intention i);
    }
}
