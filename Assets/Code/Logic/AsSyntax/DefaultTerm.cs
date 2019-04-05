using Assets.Code.Logic.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public abstract class DefaultTerm: Term
    {
        private static readonly long serialVersionUID = 1L;

        protected int? hashCodeCache = null;
        protected SourceInfo srcInfo = null;

        public virtual Term Capply(Unifier u)
        {
            return Clone();
        }

        abstract public Term Clone();
        abstract public int? CalcHashCode();

        public virtual Term CloneNS(Atom newNamespace)
        {
            return Clone();
        }

        public virtual int CompareTo(Term t)
        {
            if (t == null)
            {
                return -1;
            }
            else
            {
                return this.ToString().CompareTo(t.ToString());
            }
        }

        public void CountVars(Dictionary<VarTerm, int?> c){}

        public VarTerm GetCyclicVar()
        {
            return null;
        }

        public SourceInfo GetSrcInfo()
        {
            return srcInfo;
        }

        public void ResetHashCodeCache()
        {
            hashCodeCache = null;
        }

        public override int GetHashCode()
        {
            if (hashCodeCache == null)
            {
                hashCodeCache = CalcHashCode();
            }
            return hashCodeCache;
        }

        public bool Subsumes(Term l)
        {
            if (l.IsVar())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetSrcInfo(SourceInfo s)
        {
            srcInfo = s;
        }

        public string GetErrorMsg()
        {
            if (srcInfo == null)
            {
                return "";
            }
            else
            {
                return srcInfo.ToString();
            }
        }

        /****** Checks *******/
        public bool HasVar(VarTerm t, Unifier u)
        {
            return false;
        }

        public virtual  bool IsArithExpr()
        {
            return false;
        }

        public virtual bool IsAtom()
        {
            return false;
        }

        public virtual bool IsCyclicTerm()
        {
            return false;
        }

        public virtual bool IsGround()
        {
            return true;
        }

        public virtual bool IsInternalAction()
        {
            return false;
        }

        public virtual bool IsList()
        {
            return false;
        }

        public virtual bool IsLiteral()
        {
            return false;
        }

        public virtual bool IsNumeric()
        {
            return false;
        }

        public virtual bool IsPlanBody()
        {
            return false;
        }

        public virtual bool IsPred()
        {
            return false;
        }

        public virtual bool IsRule()
        {
            return false;
        }

        public virtual bool IsString()
        {
            return false;
        }

        public virtual bool IsStructure()
        {
            return false;
        }

        public virtual bool IsUnnamedVar()
        {
            return false;
        }

        public virtual bool IsVar()
        {
            return false;
        }
        /*****************************/

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
