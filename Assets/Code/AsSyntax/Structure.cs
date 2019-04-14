using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.Logic.AsSyntax;
using Assets.Code.ReasoningCycle;

namespace Assets.Code.AsSyntax
{
    public class Structure : Atom
    {
        public static readonly List<ITerm> emptyTermList = new List<ITerm>(0);
        public static readonly ITerm[] emptyTermArray = new ITerm[0];
        private List<ITerm> terms;
        private static readonly bool useShortUnnamedVars = Config.Get().GetBoolean(Config.SHORT_UNNAMED_VARS); //I dont really know what this is for


        public Structure(string functor) : base(DefaultNS, functor)
        {

        }

        public Structure(Literal l) : base(DefaultNS, l)
        {

        }

        public Structure(Atom namespce, Literal l) : base(namespce, l)
        {
            int tss = l.GetArity();
            if (tss > 0)
            {
                terms = new List<ITerm>(tss);
                for (int i = 0; i < tss; i++)
                {
                    terms.Add(l.GetTerm(i).Clone());
                }
            }
        }

        public Structure(Atom namespce, string functor) : base(namespce, functor)
        {

        }

        public Structure(Literal l, Unifier u) : base(l, u)
        {
            int tss = l.GetArity();
            if (tss > 0)
            {
                terms = new List<ITerm>(tss);
                for (int i = 0; i < tss; i++)
                {
                    terms.Add(l.GetTerm(i).Clone());
                }
            }
            ResetHashCodeCache();
        }

        public Structure(string functor, int termSize) : base(functor)
        {
            if (termSize > 0)
            {
                terms = new List<ITerm>(termSize);
            }
        }

        public override int CalcHashCode()
        {
            int result = base.CalcHashCode();
            int ts = GetArity();
            for (int i = 0; i < ts; i++)
            {
                result = 7 * result + GetTerm(i).GetHashCode();
            }

            return result;
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (o == this)
            {
                return true;
            }

            if (o.GetType() == typeof(Structure))
            {
                Structure oAsStructure = (Structure)o;
                if (oAsStructure.IsVar())
                {
                    return ((VarTerm)o).Equals(this);
                }
                int ts = GetArity();
                if (ts != oAsStructure.GetArity())
                {
                    return false;
                }
                if (!GetFunctor().Equals(oAsStructure.GetFunctor()))
                {
                    return false;
                }
                if (!GetNS().Equals(oAsStructure.GetNS()))
                {
                    return false;
                }
                for (int i = 0; i < ts; i++)
                {
                    if (!GetTerm(i).Equals(oAsStructure.GetTerm(i)))
                    {
                        return false;
                    }
                }
                return true;
            }
            if (o.GetType() == typeof(Atom) && this.IsAtom())
            {
                return base.Equals(o);
            }
            return false;
        }

        public override int CompareTo(ITerm t)
        {
            int c = base.CompareTo(t);
            if (c != 0)
            {
                return c;
            }
            if (t.IsStructure())
            {
                Structure s = (Structure)t;
                int ma = GetArity();
                int oa = s.GetArity();
                for (int i = 0; i < ma && i < oa; i++)
                {
                    c = GetTerm(i).CompareTo(s.GetTerm(i));
                    if (c != 0)
                    {
                        return c;
                    }
                }
            }
            return c;
        }

        public override bool Subsumes(ITerm t)
        {
            if (t.IsStructure())
            {
                Structure s = (Structure)t;
                int ma = GetArity();
                int oa = s.GetArity();
                for (int i = 0; i < ma && i < oa; i++)
                {
                    if (!GetTerm(i).Subsumes(s.GetTerm(i)))
                    {
                        return false;
                    }
                }
                return true;
            } else
            {
                return base.Subsumes(t);
            }
        }

        public override ITerm Capply(Unifier u) //I dont understand why theres an error here yay 
        {
            return new Structure(this, u);
        }

        public override ITerm Clone() //i dont know if this is override or not yay again
        {
            Structure s = new Structure(this);
            s.hashCodeCache = this.hashCodeCache;
            return s;
        }

        public override Literal CloneNS(Atom newNamespace)
        {
            return new Structure(newNamespace, this);
        }

        public override void AddTerm(ITerm t)
        {
            if (t == null)
            {
                return;
            }
            if (terms == null)
            {
                terms = new List<ITerm>(5); //Why??
            }
            terms.Add(t);
            predicateIndicatorCache = null;
            ResetHashCodeCache();
        }

        public override void DelTerm(int index)
        {
            if (terms == null)
            {
                return;
            }
            terms.RemoveAt(index);
            predicateIndicatorCache = null;
            ResetHashCodeCache();
        }

        public override Literal AddTerms(params ITerm[] ts)
        {
            if (terms == null)
            {
                terms = new List<ITerm>(5);
            }
            foreach (ITerm t in ts)
            {
                terms.Add(t);
            }
            predicateIndicatorCache = null;
            ResetHashCodeCache();
            return this;
        }

        public override Literal AddTerms(List<ITerm> l)
        {
            if (terms == null)
            {
                terms = new List<ITerm>(5);
            }
            foreach (ITerm t in l)
            {
                terms.Add(t);
            }
            predicateIndicatorCache = null;
            ResetHashCodeCache();
            return this;
        }

        public override Literal SetTerms(List<ITerm> l)
        {
            terms = l;
            predicateIndicatorCache = null;
            ResetHashCodeCache();
            return this;
        }

        public override void SetTerm(int i, ITerm t)
        {
            if (terms == null)
            {
                terms = new List<ITerm>(5);
            }
            terms.Insert(i, t);
            ResetHashCodeCache();
        }

        public override ITerm GetTerm(int i)
        {
            if (terms == null)
            {
                return null;
            } else
            {
                return terms.ElementAt(i);
            }
        }

        public override int GetArity()
        {
            if (terms == null)
            {
                return 0;
            } else
            {
                return terms.Count;
            }
        }

        public override List<ITerm> GetTerms()
        {
            return terms;
        }

        public override bool HasTerm()
        {
            return GetArity() > 0;
        }

        public override bool IsStructure()
        {
            return true;
        }

        public override bool IsAtom()
        {
            return !HasTerm();
        }

        public override bool IsGround()
        {
            int size = GetArity();
            for (int i = 0; i < size; i++)
            {
                if (!GetTerm(i).IsGround())
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsUnary()
        {
            return GetArity() == 1;
        }

        public override Literal MakeVarsAnnon()
        {
            return MakeVarsAnnon(new Unifier());
        }

        public override Literal MakeVarsAnnon(Unifier u)
        {
            int size = GetArity();
            for (int i = 0; i <size; i++)
            {
                ITerm t = GetTerm(i);
                if (t.IsVar())
                {
                    SetTerm(i, VarToReplace(t, u));
                } else if (t.GetType() == typeof(Structure))
                {
                    ((Structure)t).MakeVarsAnnon(u);
                }
            }
            ResetHashCodeCache();
            return this;
        }

        public VarTerm VarToReplace(ITerm t, Unifier u)
        {
            VarTerm v = (VarTerm)t;
            VarTerm deref = u.Deref(v);
            if (deref.Equals(v))
            {
                Atom a = v.GetNS();
                if (a.IsVar())
                {
                    a = VarToReplace(a, u);
                }
                UnnamedVar uv = useShortUnnamedVars ? new UnnamedVar(a) : UnnamedVar.Create(a, t.ToString());
                if (deref.HasAnnot())
                {
                    uv.SetAnnots(deref.GetAnnots().CloneLT());
                    uv.MakeVarsAnnon(u);
                }
                u.Bind(deref, v);
                return v;
            } else
            {
                Atom a = v.GetNS();
                if (a.IsVar())
                {
                    a = VarToReplace(a, u);
                }
                deref = (VarTerm)deref.CloneNS(a);
                if (v.HasAnnot() && !deref.HasAnnot())
                {
                    deref.SetAnnots(v.GetAnnots().CloneLT());
                    deref.MakeVarsAnnon(u);
                }
                return deref;
            }
        }

        public override void MakeTermsAnnon()
        {
            int size = GetArity();
            for (int i = 0; i < size; i++)
            {
                SetTerm(i, new UnnamedVar());
            }
            ResetHashCodeCache();
        }

        public override bool HasVar(VarTerm t, Unifier u)
        {
            int size = GetArity();
            for (int i = 0; i < size; i++)
            {
                if (GetTerm(i).HasVar(t, u))
                {
                    return true;
                }
            }
            return false;
        }

        public new List<VarTerm> GetSingletonVars()
        {
            Dictionary<VarTerm, int?> all = new Dictionary<VarTerm, int?>();
            CountVars(all);
            List<VarTerm> r = new List<VarTerm>();
            foreach (VarTerm v in all.Keys)
            {
                if (all[v] == 1 &&  !v.IsUnnamedVar())
                {
                    r.Add(v);
                }
            }
            return r;
        }

        public override void CountVars(Dictionary<VarTerm, int?> c)
        {
            int tss = GetArity();
            for (int i = 0; i < tss; i++)
            {
                GetTerm(i).CountVars(c);
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            if (GetNS() != DefaultNS)
            {
                s.Append(GetNS());
                s.Append("::");
            }
            if (Negated())
            {
                s.Append("~");
            }
            if (GetFunctor() != null)
            {
                s.Append(GetFunctor());
            }
            if (GetArity() > 0)
            {
                s.Append("(");
                IEnumerator<ITerm> enumerator = terms.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    s.Append(enumerator.Current);
                    if(enumerator.MoveNext())
                    {
                        s.Append(",");
                    }
                }
                s.Append(")");
            }
            if (HasAnnot())
            {
                s.Append(GetAnnots().ToString());
            }
            return s.ToString();
        }
    }
}
