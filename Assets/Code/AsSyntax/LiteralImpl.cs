using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    public class LiteralImpl : Pred
    {
        private static readonly long serialVersionUID = 1L;
        private bool type = LPos;

        /** creates a positive literal */
        public LiteralImpl(string functor):base(functor)
        {
        }

        /** if pos == true, the literal is positive, otherwise it is negative */
        public LiteralImpl(bool pos, string functor) : base(functor)
        {
            type = pos;
        }

        public LiteralImpl(Literal l):base(l)
        {
            type = !l.Negated();
        }

        // used by capply
        public LiteralImpl(Literal l, Unifier u) : base(l, u)
        {
            type = !l.Negated();
        }

        /** if pos == true, the literal is positive, otherwise it is negative */
        public LiteralImpl(Literal l, bool pos, string v) : base(l)
        {
            type = pos;
        }

        /** if pos == true, the literal is positive, otherwise it is negative */
        public LiteralImpl(Atom @namespace, bool pos, string functor): base(@namespace, functor)
        {
            type = pos;
        }

        /** creates a literal based on another but in another name space and signal */
        public LiteralImpl(Atom @namespace, bool pos, Literal l):base(@namespace, l)
        {
            type = pos;
        }

        public override bool IsAtom()
        {
            return base.IsAtom() && !Negated();
        }

        public override bool CanBeAddedInBB()
        {
            return true;
        }

        public override bool Negated()
        {
            return type == LNeg;
        }

        public new Literal SetNegated(bool b)
        {
            type = b;
            ResetHashCodeCache();
            return this;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o == this) return true;

            if (o.GetType() == typeof(LiteralImpl)) {
                LiteralImpl l = (LiteralImpl)o;
                return type == l.type && GetHashCode() == l.GetHashCode() && base.Equals(l);
            } else if (o.GetType() == typeof(Atom) && !Negated()) {
                return base.Equals(o);
            }
            return false;
        }

        public override string GetErrorMsg()
        {
            string src = GetSrcInfo() == null ? "" : " (" + GetSrcInfo() + ")";
            return "Error in '" + this + "'" + src;
        }

        public override int CompareTo(ITerm t)
        {
            if (t == null)
                return -1;
            if (t.IsLiteral())
            {
                Literal tl = (Literal)t;
                if (!Negated() && tl.Negated())
                    return -1;
                else if (Negated() && !tl.Negated())
                    return 1;
            }
            return base.CompareTo(t);
        }

        public override ITerm Clone()
        {
            Literal l = new LiteralImpl(this);
            l.hashCodeCache = hashCodeCache;
            return l;
        }

        public override ITerm Capply(Unifier u)
        {
            return new LiteralImpl(this, u);
        }

        public override Literal CloneNS(Atom newNamespace)
        {
            return new LiteralImpl(newNamespace, !Negated(), this);
        }

        public override int? CalcHashCode()
        {
            int? result = base.CalcHashCode();
            if (Negated())
                result += 32371;
            return result;
        }

        /** returns [~] super.getPredicateIndicator */
        public override PredicateIndicator GetPredicateIndicator()
        {
            if (predicateIndicatorCache == null)
                predicateIndicatorCache = new PredicateIndicator(GetNS(), ((type == LPos) ? GetFunctor() : "~" + GetFunctor()), GetArity());
            return predicateIndicatorCache;
        }
    }
}
