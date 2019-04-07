namespace Assets.Code.AsSyntax
{

    public interface IPlanBody : ITerm
    {
        BodyType GetBodyType();
        ITerm GetBodyTerm();
        IPlanBody GetBodyNext();
        IPlanBody GetHead();

        bool IsEmptyBody();
        int GetPlanSize();

        void SetBodyType(BodyType bt);
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

    public class BodyType
    {
        internal static BodyType none;
    }
}
