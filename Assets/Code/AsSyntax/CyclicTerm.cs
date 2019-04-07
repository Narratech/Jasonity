using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    /**
     * A term with recursion (cyclic), created by code like X = f(X).
     */
    public class CyclicTerm:LiteralImpl
    {
        private static readonly long serialVersionUID = 1L;

        private VarTerm cyclicVar = null;

        public CyclicTerm(Literal t, VarTerm v): base(t)
        {
            cyclicVar = v;
        }

        public CyclicTerm(Literal t, VarTerm v, Unifier u): base(t,u)
        {
            cyclicVar = v;
        }

        public VarTerm GetCyclicVar()
        {
            return cyclicVar;
        }
        public bool IsCyclicTerm()
        {
            return true;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o == this) return true;

            if (o.GetType() == typeof(CyclicTerm))
            {
                CyclicTerm l = o as CyclicTerm;
                return base.Equals(l);
            }
            return false;
        }

        public Literal MakeVarsAnnon(Unifier u)
        {
            base.MakeVarsAnnon(u);
            VarTerm v = u.Deref(cyclicVar);
            if (v != null)
            {
                cyclicVar = v;
            }
            return this;
        }

        public ITerm Capply(Unifier u)
        {
            ITerm v = u.Remove(cyclicVar);
            ITerm r = new CyclicTerm(this, cyclicVar.Clone() as VarTerm, u);
            if (v != null)
            {
                u.Bind(cyclicVar, v);
            }
            return r;
        }

        public ITerm Clone()
        {
            return new CyclicTerm(this, cyclicVar.Clone() as VarTerm);
        }

        protected int? CalcHashCode()
        {
            return base.CalcHashCode() + cyclicVar.CalcHashCode();
        }

        public override string ToString()
        {
            return "..." + base.ToString() + "/" + cyclicVar;
        }
    }
}
