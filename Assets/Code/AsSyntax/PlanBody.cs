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
        public readonly static BodyType none;
        public readonly static BodyType action;
        public readonly static BodyType internalAction;
        public readonly static BodyType achieve;
        public readonly static BodyType test;
        public readonly static BodyType addBel;
        public readonly static BodyType addBelNewFocus;
        public readonly static BodyType addBelBegin;
        public readonly static BodyType addBelEnd;
        public readonly static BodyType delBel;
        public readonly static BodyType delBelNewFocus;
        public readonly static BodyType delAddBel;
        public readonly static BodyType achieveNF;
        public readonly static BodyType constraint;

        public override string ToString()
        {
            if (this == none || this == action || this == internalAction || this == constraint) return "";
            else if (this == achieve) return "!";
            else if (this == test) return "?";
            else if (this == addBel) return "+";
            else if (this == addBelNewFocus) return "++";
            else if (this == addBelBegin) return "+<";
            else if (this == addBelEnd) return "+>";
            else if (this == delBel) return "-";
            else if (this == delBelNewFocus) return "--";
            else if (this == delAddBel) return "-+";
            else if (this == achieveNF) return "!!";
            else return null;
        }
    }
}
