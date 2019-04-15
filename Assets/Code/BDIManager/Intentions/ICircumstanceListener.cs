using BDIManager.Intentions;

namespace Assets.Code.BDIManager
{
    public interface ICircumstanceListener 
    {
        void EventAdded(Event e);

        void IntentionAdded(Intention i);

        void IntentionDropped(Intention i);

        void IntentionResumed(Intention i);

        void IntentionSuspended(Intention i, string reason);
    }
}
