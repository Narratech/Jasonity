using Assets.Code.Logic.AsSyntax;

namespace Assets.Code.Logic
{
    /*Represent a binary/unary logical/relational operator*/
    public abstract class BinaryStructure:Structure
    {
        /*Constructor for binary operator*/
        public BinaryStructure(Term t1, string id, Term t2):base(id,2)
        {
            AddTerm(t1);
            AddTerm(t2);
            if (t1.GetSrcInfo() != null)
                srcInfo = t1.GetSrcInfo();
            else
                srcInfo = t2.GetSrcInfo();
        }

        /*Constructor for unary operator*/
        public BinaryStructure(string id, Term arg):base(id, 1)
        {
            AddTerm(arg);
            srcInfo = arg.GetSrcInfo();
        }

        /*Gets the LHS of this operation*/
        public Term GetLHS()
        {
            return GetTerm(0);
        }

        /*Gets the RHS of this operation*/
        public Term GetRHS()
        {
            return GetTerm(1);
        }

        public override string ToString()
        {
            if (IsUnary())
                return GetFunctor() + "(" + GetTerm(0) + ")";
            else
                return "(" + GetTerm(0) + GetFunctor() + GetTerm(1) + ")";
        }
    }
}