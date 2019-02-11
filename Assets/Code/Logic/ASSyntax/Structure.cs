using Logica.ASSemantic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/**
 * Represents a structure: a functor with <i>n</i> arguments,
 * e.g.: val(10,x(3)).
 */
namespace Jason.Logic.AsSyntax
{
    public class Structure: Atom
    {
        private static readonly long serialVersionUID = 1L;

        protected static readonly List<Term> emptyTermList  = new List<Term>(0);
        protected static readonly Term[]     emptyTermArray = new Term[0]; // just to have a type for toArray in the getTermsArray method

        private List<Term> terms;
        //protected Boolean isGround = true; // it seems to not improve the performance

        public Structure(string functor): this(DefaultNS, functor) { }

        public Structure(Literal l): this(l.GetNS(), l) { }

        public Structure(Atom @namespace, string functor): base(@namespace, functor) { }

        public Structure(Atom @namespace, Literal l): base(@namespace, l)
        {
            int tss = l.GetArity();
            if(tss > 0)
            {
                terms = new List<Term>(tss);
                for(int i = 0; i < tss; i++)
                {
                    terms.Add(l.GetTerm(i).Clone());
                }
            }
            ResetHashCodeCache();
        }

        //used by capply
        protected Structure(Literal l, Unifier u): base(l, u)
        {
            int tss = l.GetArity();
            if(tss > 0)
            {
                terms = new List<Term>(tss);
                for(int i = 0; i < tss; i++)
                {
                    terms.Add(l.GetTerm(i).Capply(u));
                }
            }
            ResetHashCodeCache();
        }

        /**
         * Create a structure with a defined number of terms.
         *
         * It is used by list term, plan body, ... to not create the array list for terms.
         */
        public Structure(string functor, int termSize) : base(functor)
        {
            if (termSize > 0)
                terms = new List<Term>(termSize);
        }

        public static Structure parse(string sTerm)
        {
            as2j parser = new as2j(new StringReader(sTerm));
            try
            {
                Term t = parser.term();
                if (t.GetType() == typeof(Structure))
                    return (Structure)t;
                else
                    return new Structure((Atom)t);
            }
            catch(Exception)
            {
                Debug.Log("Error parsing structure " + sTerm);
                return null;
            }
        }

        protected override int CalcHashCode()
        {
            int result = base.CalcHashCode();
            int ts = GetArity();
            for (int i = 0; i < ts; i++)
                result = 7 * result + GetTerm(i).GetHashCode();
            return result;
        }

        public bool Equals(object t)
        {
            if (t == null) return false;
            if (t == this) return true;

            if(t.GetType() == typeof(Structure))
            {
                Structure tAsStruct = (Structure)t;

                // if t is a VarTerm, uses var's equals
                if (tAsStruct.IsVar())
                    return ((VarTerm)t).Equals(this);

                int ts = GetArity();
                if (ts != tAsStruct.GetArity())
                    return false;

                if (!GetFunctor().Equals(tAsStruct.GetFunctor()))
                    return false;
                if (!GetNS().Equals(tAsStruct.GetFunctor()))
                    return false;
                for(int i = 0; i < ts; i++)
                {
                    if (!GetTerm(i).Equals(tAsStruct.GetTerm(i)))
                        return false;
                }
                return true;
            }
            if (t.GetType() == typeof(Atom) && this.IsAtom())
                return base.Equals(t);
            return false;
        }

        public int CompareTo(Term t)
        {
            int c = base.CompareTo(t);
            if (c != 0)
                return c;
            if (t.IsStructure())
            {
                Structure tAsStruct = (Structure)t;
                int ma = GetArity();
                int oa = tAsStruct.GetArity();
                for(int i = 0; i < ma && i < oa; i++)
                {
                    c = GetTerm(i).CompareTo(tAsStruct.GetTerm(i));
                    if (c != 0)
                        return 0;
                }
            }
            return 0;
        }

        public bool Subsumes(Term t)
        {
            if (t.IsStructure())
            {
                Structure tAsStruct = (Structure)t;

                int ma = GetArity();
                int oa = tAsStruct.GetArity();
                for(int i = 0; i < ma && i < oa; i++)
                {
                    if (!GetTerm(i).Subsumes(tAsStruct.GetTerm(i)))
                        return false;
                }
                return true;
            }
            else
            {
                return base.Subsumes(t);
            }
        }

        public Term Capply(Unifier u)
        {
            return new Structure(this, u);
        }

        /** make a deep copy of the terms */
        public Term Clone()
        {
            Structure s = new Structure(this);
            s.hashCodeCache = this.hashCodeCache;
            return s;
        }

        public Literal CloneNS(Atom newnamespace)
        {
            return new Structure(newnamespace, this);
        }

        public void AddTerm(Term t)
        {
            if (t == null) return;
            if (terms == null) terms = new List<Term>(5);
            terms.Add(t);
            predicateIndicatorCache = null;
            ResetHashCodeCache();
        }

        public void DelTerm(int index)
        {
            if (terms == null)
                return;
            terms.RemoveAt(index);
            predicateIndicatorCache = null;
            ResetHashCodeCache();
        }

        public Literal AddTerms(Term ts)
        {
            if (terms == null)
                terms = new List<Term>(5);
            foreach(Term t in ts)
                terms.Add(t);
            predicateIndicatorCache = null;
            ResetHashCodeCache();
            return this;
        }

        public Literal AddTerms(List<Term> l)
        {
            if (terms == null)
                terms = new List<Term>(5);
            foreach (Term t in l)
                terms.Add(t);
            predicateIndicatorCache = null;
            ResetHashCodeCache();
            return this;
        }

        public Literal SetTerms(List<Term> l)
        {
            terms = l;
            predicateIndicatorCache = null;
            ResetHashCodeCache();
            return this;
        }

        public void SetTerm(int i, Term t)
        {
            if (terms == null)
                terms = new List<Term>(5);
            terms.Insert(i, t);
            ResetHashCodeCache();
        }

        public Term GetTerm(int i)
        {
            if (terms == null)
                return null;
            else
                return terms.ElementAt(i);
        }

        public int GetArity()
        {
            if (terms == null)
                return 0;
            else
                return terms.Count();
        }

        public List<Term> GetTerms()
        {
            return terms;
        }

        public bool HasTerm()
        {
            return GetArity() > 0;
        }

        public bool IsStructure()
        {
            return true;
        }

        public bool IsAtom()
        {
            return !HasTerm();
        }

        public bool IsGround()
        {
            int size = GetArity();
            for(int i = 0; i < size; i++)
            {
                if (!GetTerm(i).IsGround())
                    return false;
            }
            return true;
        }

        public bool IsUnary()
        {
            return GetArity() == 1;
        }

        public Literal MakeVarsAnnon()
        {
            return MakeVarsAnnon(new Unifier());
        }

        public Literal MakeVarsAnnon(Unifier un)
        {
            int size = GetArity();
            for(int i = 0; i < size; i++)
            {
                Term ti = GetTerm(i);
                if (ti.IsVar())
                    SetTerm(i, VarToReplace(ti, un));
                else if (ti.GetType() == typeof(Structure))
                    ((Structure)ti).MakeVarsAnnon(un);
            }
            ResetHashCodeCache();
            return this;
        }

        private static bool UseShortUnnamedVars = Config.get().getBoolean(Config.SHORT_UNNAMED_VARS);

        public VarTerm VarToReplace(Term t, Unifier un)
        {
            VarTerm vt = (VarTerm)t;
            VarTerm deref = un.Deref(vt);

            // if the variable hasn't been renamed given the input unifier, then rename it.
            if (deref.Equals(vt))
            {
                // forget the name
                Atom ns = vt.GetNS();
                if (ns.IsVar())
                    ns = VarToReplace(ns, un);
                UnnamedVar var = UseShortUnnamedVars ? new UnnamedVar(ns) : UnnamedVar.Create(ns, t.ToString());

                // if deref has annotations then we need to replicate these in the new variable
                if (deref.HasAnnot()) 
                {
                    var.SetAnnots(deref.GetAnnots().CloneLT());
                    var.MakeVarsAnnon(un);
                }
                un.Bind(deref, var);
                return var;
            }
            else
            {
                // otherwise it has already been renamed in this scope so return
                // the existing renaming
                Atom ns = vt.GetNS();
                if (ns.IsVar())
                    ns = VarToReplace(ns, un);
                deref = (VarTerm)deref.CloneNS(ns);
                // ensure that if the input term has an annotation and the existing
                // renaming doesn't then we add the anonymized annotations
                if(vt.HasAnnot() && !deref.HasAnnot())
                {
                    deref.SetAnnots(vt.GetAnnots().CloneLT());
                    deref.MakeVarsAnnon(un);
                }
                return deref;
            }
        }

        public void MakeTermsAnnon()
        {
            int size = GetArity();
            for(int i = 0; i < size; i++)
                SetTerm(i, new UnnamedVar());
            ResetHashCodeCache();
        }

        public bool HasVar(VarTerm t, Unifier u)
        {
            int size = GetArity();
            for(int i = 0; i < size; i++)
            {
                if (GetTerm(i).HasVar(t, u))
                    return true;
            }
            return false;
        }

        public List<VarTerm> GetSingletonVars()
        {
            Dictionary<VarTerm, int?> all = new Dictionary<VarTerm, int?>();
            CountVars(all);
            List<VarTerm> r = new List<VarTerm>();
            foreach (VarTerm k in all.Keys)
            {
                if (all.Item[k] == 1 && !k.IsUnnamedVar())
                    r.Add(k);
            }
            return r;
        }

        public void CountVars(Dictionary<VarTerm, int?> c)
        {
            int tss = GetArity();
            for(int i = 0; i < tss; i++)
            {
                GetTerm(i).CountVars(c);
            }
        }

        public string ToString()
        {
            StringBuilder s = new StringBuilder();
            if (GetNS() != DefaultNS)
            {
                s.Append(GetNS());
                s.Append("(::)");
            }
            if (Negated())
                s.Append("~");
            if (GetFunctor() != null)
                s.Append(GetFunctor());
            if (GetArity() > 0)
            {
                s.Append('(');
                IEnumerator<Term> i = terms.Iterator();
                while (i.HasNext())
                {
                    s.Append(i.MoveNext());
                    if (i.HasNext())
                        s.Append(',');
                }
                s.Append(')');
            }
            if (HasAnnot())
                s.Append(GetAnnots().ToString());
            return s.ToString();               
        }
    }
}
