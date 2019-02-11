using Logica.ASSemantic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Represents an atom (a positive literal with no argument and no annotation, e.g. "tell", "a").
 */

namespace Jason.Logic.AsSyntax
{
    public class Atom : Literal
    {
        private static readonly long serialVersionUID = 1L;

        private readonly string functor; // immutable field
        private readonly Atom   ns; // name space

        public Atom(string functor) : this(DefaultNS, functor)
        {
        }

        protected Atom(Atom @namespace, string functor)
        {
            this.ns = @namespace;
            this.functor = functor;
        }

        public Atom(Literal l, Unifier u): this((Atom)l.GetNS().Capply(u), l)
        {
        }

        public Atom(Atom @namespace, Literal l)
        {
            this.functor = l.GetFunctor();
            this.ns = @namespace;
            //predicateIndicatorCache = l.predicateIndicatorCache;
            //hashCodeCache           = l.hashCodeCache;
            srcInfo = l.GetSrcInfo();
        }

        public Term Clone()
        {
            return this; //the object is inmutable
        }

        public override string GetFunctor()
        {
            return functor;
        }

        public override Atom GetNS()
        {
            return ns;
        }

        public Term Capply(Unifier u)
        {
            if (ns.IsVar())
                return new Atom(this, u);
            else
                return this;
        }

        public Literal CloneNS(Atom newnamespace)
        {
            return new Atom(newnamespace, this);
        }

        public bool IsAtom()
        {
            return true;
        }

        public bool Equals(object o)
        {
            if (o == null) return false;
            if (o == this) return true;
            if (o.GetType() == typeof(Atom))
            {
                Atom a = (Atom)o;
                return a.IsAtom() &&
                    GetFunctor().Equals(a.GetFunctor()) &&
                    GetNS().Equals(a.GetNS());
            }
            return false;
        }

        public int CompareTo(Term t)
        {
            if (t == null) return -1;
            if (t.IsNumeric()) return 1;

            // this is a list and the other not
            if (IsList() && !t.IsList()) return -1;

            // this is not a list and the other is
            if (!IsList() && t.IsList()) return 1;

            // both are lists, check the size
            if (IsList() && t.IsList())
            {
                ListTerm l1 = (ListTerm)this;
                ListTerm l2 = (ListTerm)t;
                int l1s = l1.Count();
                int l2s = l2.Count();
                if (l1s > l2s) return 1;
                if (l2s > l1s) return -1;
                return 0; // need to check elements (in Structure class)
            }
            if (t.IsVar())
                return -1;
            if(t.GetType() == typeof(Literal))
            {
                Literal tAsLit = (Literal)t;
                if (GetNS().Equals(tAsLit.GetNS()))
                {
                    int ma = GetArity();
                    int oa = tAsLit.GetArity();
                    if(ma < oa)
                    {
                        return -1;
                    }
                    else if(ma > oa)
                    {
                        return 1;
                    }
                    else
                    {
                        return GetNS().CompareTo(tAsLit.GetFunctor());
                    }
                }
                else
                {
                    return GetNS().CompareTo(tAsLit.GetNS());
                }
            }
            return base.CompareTo(t);
        }

        protected override int CalcHashCode()
        {
            return GetFunctor().GetHashCode() + GetNS().HashCode();
        }

        public string ToString()
        {
            if (ns == DefaultNS)
                return functor;
            else
                return GetNS() + "::" + functor;
        }

        public override Term Clone()
        {
            throw new NotImplementedException();
        }
    }
}
