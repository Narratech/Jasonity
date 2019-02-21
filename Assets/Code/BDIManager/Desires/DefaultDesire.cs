using System;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using BDIManager.Intentions;

//Implements the Desire interface
namespace BDIManager.Desires {
    class DefaultDesire : Desire
    {
        public DefaultDesire()
        {
        }

        private Reasoner reasoner;


        public DefaultDesire(Reasoner r)
        {
            reasoner = r;
        }




        public void DesireFailed(Trigger desire)
        {
            GenerateDesireStateEvent();
        }

        public void DesireFinished(Trigger desire, FinishStates result)
        {
            GenerateDesireStateEvent();
        }

        public void DesireResumed(Trigger desire)
        {
            GenerateDesireStateEvent();
        }

        public void DesireStarted(Event desire)
        {
            GenerateDesireStateEvent();
        }

        public void DesireSuspended(Trigger desire, string reason)
        {
            GenerateDesireStateEvent();
        }

        private void GenerateDesireStateEvent(Literal desire, DesireStates state, String reason )
        {
            reasoner.RunAtBeginOfNextCycle();
            reasoner.GetUserAgArch().WakeUpDeliberate();
        }
    }
}
