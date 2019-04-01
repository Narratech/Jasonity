using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic.AsSyntax
{
    public class NumberTermImpl : DefaultTerm, NumberTerm
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

        protected override int? CalcHashCode()
        {
            throw new NotImplementedException();
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

            if (o != null && (o.GetType() == typeof(Term)) && (o.IsNumeric() as Term && o.IsArithExpr() as Term))
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

        public override int CalcHashCode()
        {
            return 37 * value as int;
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
            long r = Math.Round(value);
            if (value == r as double)
            {
                return string.ValueOf(r);
                //return r.ToString();
            }
            else
            {
                return string.ValueOf(value);
                //return value.ToString();
            }
        }
    }
}
