using System;
using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;
using Assets.Code.Utilities;
using BDIManager.Intentions;

//Implements the Desire interface
namespace BDIManager.Desires {
    public class Desire
    {
        public enum DesireStates { started, suspended, resumed, finished, failed };
        public enum FinishStates { achieved, unachieved, dropped };

        Reasoner reasoner;

        public Desire(Reasoner r)
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
            reasoner.RunAtBeginOfNextCycle(new RunnableImpl(desire, state, reason, type, reasoner));
            reasoner.GetUserAgArch().WakeUpDeliberate();
        }

        private class RunnableImpl : IRunnable
        {
            Literal desire;
            DesireStates state;
            string reason;
            TEType type;
            Reasoner reasoner;

            public RunnableImpl(Literal desire, DesireStates state, string reason, TEType type, Reasoner reasoner)
            {
                this.desire = desire;
                this.state = state;
                this.reason = reason;
                this.type = type;
                this.reasoner = reasoner;
            }

            public void Run()
            {
                Literal newDesire = desire.ForceFullLiteralImpl().Copy();
                Literal stateAnnot = AsSyntax.CreateLiteral("state", new Atom(state.ToString()));
                if (reason != null)
                {
                    stateAnnot.AddAnnot(AsSyntax.CreateStructure("reason", new StringTermImpl(reason)));
                }
                newDesire.AddAnnot(stateAnnot);
                Trigger eEnd = new Trigger(TEOperator.goalState, type, newDesire);
                if (reasoner.GetAgent().GetPL().HasCandidatePlan(eEnd))
                {
                    reasoner.GetCircumstance().InsertMetaEvent(new Event(eEnd, null));
                }
            }
        }

        public object AllDesires(Circumstance circumstance, Literal body, object p, Unifier unifier)
        {
            throw new NotImplementedException(); 
        }
    }
}
