using System;
using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;

//Implements the Desire interface
namespace BDIManager.Desires {
    class DefaultDesire : IDesire
    {
        Reasoner reasoner;
        
        public DefaultDesire(Reasoner r)
        {
            reasoner = r;
        }

        public void DesireStarted(Event desire)
        {
            GenerateDesireStateEvent(desire.GetTrigger().GetLiteral(), TEType.achieve, DesireStates.started, null);
        }

        public void DesireFailed(Trigger desire)
        {
            GenerateDesireStateEvent(desire.GetLiteral(), desire.GetTEType(), DesireStates.failed, null);
        }

        public void DesireFinished(Trigger desire, FinishStates result)
        {
            GenerateDesireStateEvent(desire.GetLiteral(), desire.GetTEType(), DesireStates.finished, result.ToString());
        }

        public void DesireResumed(Trigger desire)
        {
            GenerateDesireStateEvent(desire.GetLiteral(), desire.GetTEType(), DesireStates.resumed, null);
        }
        
        public void DesireSuspended(Trigger desire, string reason)
        {
            GenerateDesireStateEvent(desire.GetLiteral(), desire.GetTEType(), DesireStates.suspended, reason);
        }

        private void GenerateDesireStateEvent(Literal desire, TEType type, DesireStates state, String reason)
        {
            reasoner.RunAtBeginOfNextCycle();
            reasoner.GetUserAgArch().WakeUpDeliberate();
        }
    }
}
