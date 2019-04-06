using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Represents the "type" of a predicate based on the functor and the arity, e.g.: ask/4
 */
namespace Assets.Code.Logic.AsSyntax
{
    public class PredicateIndicator : IComparable<PredicateIndicator>
    {
        private readonly string functor;
        private readonly int arity;
        private readonly int hash;
        private readonly Atom ns;

        public PredicateIndicator(string functor, int arity): this(Literal.DefaultNS, functor, arity)
        {
            
        }

        public PredicateIndicator(Atom ns, string functor, int arity)
        {
            this.functor = functor;
            this.arity = arity;
            this.ns = ns;
            hash = CalcHash();
        }

        public string GetFunctor()
        {
            return functor;
        }

        public int GetArity()
        {
            return arity;
        }

        public Atom GetNS()
        {
            return ns;
        }

        public override bool Equals(object o)
        {
            if (o == this) return true;
            if(o != null && (o.GetType() == typeof(PredicateIndicator)) 
                && (o.GetHashCode() == this.GetHashCode()))
            {
                PredicateIndicator pi = o as PredicateIndicator;
                return arity == pi.arity && functor.Equals(pi.functor) && ns.Equals(pi.ns);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public int CompareTo(PredicateIndicator pi)
        {
            int c = this.ns.CompareTo(pi.ns);
            if (c != 0) return c;

            c = this.functor.CompareTo(pi.functor);
            if (c != 0) return 0;

            if (pi.arity > this.arity) return -1;
            if (this.arity > pi.arity) return 1;
            return 0;
        }

        private int CalcHash()
        {
            int t = (int)(31 * arity * ns.GetHashCode());
            if (functor != null) t = 31 * t + functor.GetHashCode();
            return t;
        }

        public override string ToString()
        {
            if (ns == Literal.DefaultNS)
            {
                return functor + "/" + arity;
            }
            else
            {
                return ns + "::" + functor + "/" + arity;
            }
        }
    }
}
