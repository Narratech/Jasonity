namespace Assets.Code.AsSyntax
{
    /*Represent a binary/unary logical/relational operator*/
    public abstract class BinaryStructure:Structure
    {
        /*Constructor for binary operator*/
        public BinaryStructure(ITerm t1, string id, ITerm t2):base(id,2)
        {
            AddTerm(t1);
            AddTerm(t2);
            if (t1.GetSrcInfo() != null)
                srcInfo = t1.GetSrcInfo();
            else
                srcInfo = t2.GetSrcInfo();
        }

        /*Constructor for unary operator*/
        public BinaryStructure(string id, ITerm arg):base(id, 1)
        {
            AddTerm(arg);
            srcInfo = arg.GetSrcInfo();
        }

        /*Gets the LHS of this operation*/
        public ITerm GetLHS()
        {
            return GetTerm(0);
        }

        /*Gets the RHS of this operation*/
        public ITerm GetRHS()
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