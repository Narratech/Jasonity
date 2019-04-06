using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic.AsSyntax
{
    /** Immutable class that implements a term that represents a number */
    public sealed class NumberTermImpl : DefaultTerm, NumberTerm
    {
        private static readonly long serialVersionUID = 1L;

        private readonly double value;

        public NumberTermImpl(): base()
        {
            value = 0;
        }

        public NumberTermImpl(double vl)
        {
            value = vl;
        }

        public NumberTermImpl(NumberTermImpl t)
        {
            value = t.value;
            srcInfo = t.srcInfo;
        }

        public double Solve()
        {
            return value;
        }

        public override Term Clone()
        {
            return this;
        }

        public override bool IsNumeric()
        {
            return true;
        }

        public override bool Equals(object o)
        {
            if (o == this) return true;

            if (o != null && (o.GetType() == typeof(Term)) && (((Term)o).IsNumeric() && ((Term)o).IsArithExpr()))
            {
                NumberTerm st = o as NumberTerm;
                try
                {
                    return Solve() == st.Solve();
                }
                catch(Exception e) { }
            }
            return false;
        }

        public override int? CalcHashCode()
        {
            return (int?)(37 * value);
        }

        public override int CompareTo(Term o)
        {
            if (o.GetType() == typeof(VarTerm))
            {
                return o.CompareTo(this) * -1;
            }
            if (o.GetType() == typeof(NumberTermImpl))
            {
                NumberTermImpl st = o as NumberTermImpl;
                if (value > st.value) return 1;
                if (value < st.value) return -1;
                return 0;
            }
            return -1;
        }

        public override string ToString()
        {
            long r = (long)Math.Round(value);
            if (value == (double)r)
            {
                return r.ToString();
                //return r.ToString();
            }
            else
            {
                return value.ToString();
                //return value.ToString();
            }
        }
    }
}
