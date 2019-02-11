using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logica.ASSemantic;

/**
 * Base class for all terms.
 *
 * (this class may be renamed to AbstractTerm in future releases of Jason, so
 * avoid using it -- use ASSyntax class to create new terms)
 *
 * @see ASSyntax
 */
namespace Jason.Logic.AsSyntax
{
    [Serializable]
    public abstract class DefaultTerm : Term
    {
        private static readonly long serialVersionUID = 1L;

        protected int? hashCodeCache = null;
        protected SourceInfo srcInfo = null;


        public Term Capply(Unifier u)
        {
            return Clone();
        }

        public abstract Term Clone();
        protected abstract int CalcHashCode();

        public int? HashCode()
        {
            if (hashCodeCache == null)
                hashCodeCache = CalcHashCode();
            return hashCodeCache;
        }

        public void ResetHashCodeCache()
        {
            hashCodeCache = null;
        }

        public Term CloneNS(Atom newnamespace)
        {
            return Clone();
        }

        public int CompareTo(Term other)
        {
            if (other == null)
                return -1;
            else
                return this.ToString().CompareTo(other.ToString());
        }

        public void CountVars(Dictionary<VarTerm, int?> c)
        {
 
        }

        //Me obliga visual a implementarlo
        public bool Equals(object o)
        {
            throw new NotImplementedException();
        }

        public VarTerm GetCyclicVar()
        {
            return null;
        }

        public SourceInfo GetSrcInfo()
        {
            return srcInfo;
        }

        public bool HasVar(VarTerm t, Unifier u)
        {
            return false;
        }

        public bool IsArithExpr()
        {
            return false;
        }

        public bool IsAtom()
        {
            return false;
        }

        public bool IsCyclicTerm()
        {
            return false;
        }

        public bool IsGround()
        {
            return true;
        }

        public bool IsInternalAction()
        {
            return false;
        }

        public bool IsList()
        {
            return false;
        }

        public bool IsLiteral()
        {
            return false;
        }

        public bool IsNumeric()
        {
            return false;
        }

        public bool IsPlanBody()
        {
            return false;
        }

        public bool IsPred()
        {
            return false;
        }

        public bool IsRule()
        {
            return false;
        }

        public bool IsString()
        {
            return false;
        }

        public bool IsStructure()
        {
            return false;
        }

        public bool IsUnnamedVar()
        {
            return false;
        }

        public bool IsVar()
        {
            return false;
        }

        public void SetSrcInfo(SourceInfo s)
        {
            srcInfo = s;
        }

        public bool Subsumes(Term l)
        {
            if (l.IsVar())
                return false;
            else
                return true;
        }

        public string getErrorMsg()
        {
            if (srcInfo == null)
                return "";
            else
                return srcInfo.ToString();
        }

        //Me obliga Visual a implementarlo
        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }
}
