using static Assets.Code.AsSyntax.PlanBodyImpl;

namespace Assets.Code.AsSyntax
{

    public interface IPlanBody : ITerm
    {
        BodyType.Body_Type GetBodyType();
        ITerm GetBodyTerm();
        IPlanBody GetBodyNext();
        IPlanBody GetHead();

        bool IsEmptyBody();
        int GetPlanSize();

        void SetBodyType(BodyType.Body_Type bt);
        void SetBodyTerm(ITerm t);
        void SetBodyNext(IPlanBody bl);
        IPlanBody GetLastBody();

        bool IsBodyTerm();
        void SetAsBodyTerm(bool b);

        bool Add(IPlanBody bl);
        bool Add(int index, IPlanBody bl);
        ITerm RemoveBody(int index);

        // Clones the plan body
        IPlanBody ClonePB();
    }
}
