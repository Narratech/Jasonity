using Assets.Code.Logic;
using BDIManager.Desires;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*This one maybe is the DefaulDesire, but I dont know yet*/
namespace Assets.Code.BDIManager.Desires
{
    class DesireListenerForMetaEvents : Desire
    {
        public void DesireFailed(Trigger desire)
        {
            throw new NotImplementedException();
        }

        public void DesireFinished(Trigger desire, FinishStates result)
        {
            throw new NotImplementedException();
        }

        public void DesireResumed(Trigger desire)
        {
            throw new NotImplementedException();
        }

        public void DesireStarted(Event desire)
        {
            throw new NotImplementedException();
        }

        public void DesireSuspended(Trigger desire, string reason)
        {
            throw new NotImplementedException();
        }
    }
}
