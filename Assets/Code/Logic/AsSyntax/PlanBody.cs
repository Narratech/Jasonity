using Assets.Code.Logic.AsSyntax;

namespace Assets.Code.Logic
{

    public interface PlanBody : Term
    {
        BodyType GetBodyType();
        Term GetBodyTerm();
        PlanBody GetBodyNext();
        PlanBody GetHead();

        bool IsEmptyBody();
        int GetPlanSize();

        void SetBodyType(BodyType bt);
        void SetBodyTerm(Term t);
        void SetBodyNext(PlanBody bl);
        PlanBody GetLastBody();

        bool IsBodyTerm();
        void SetAsBodyTerm(bool b);

        bool Add(PlanBody bl);
        bool Add(int index, PlanBody bl);
        Term RemoveBody(int index);

        // Clones the plan body
        PlanBody ClonePB();
    }

    public class BodyType
    {
        internal static BodyType none;
    }
}
