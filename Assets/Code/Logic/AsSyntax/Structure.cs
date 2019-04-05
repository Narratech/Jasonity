using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.Logic.AsSyntax;
using Assets.Code.ReasoningCycle;

namespace Assets.Code.Logic
{
    class Structure : Atom
    {
        private static List<Term> emptyTermList = new List<Term>(0);
        private static Term[] emptyTermArray = new Term[0];
        private List<Term> terms;

        public Structure(string functor) :base(DefaultNS, functor)
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
                terms = new List<Term>(tss);
                for(int i = 0; i < tss; i++)
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
                terms = new List<Term>(tss);
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
                terms = new List<Term>(termSize);
            }
        }

        public static Structure Parse(string sTerm)
        {
            as2j parser = new as2j(new StringReader(sTerm)); 
            try
            {
                Term t = parser.Term();
                if (t.GetType() == typeof(Structure))
                {
                    return (Structure)t;
                } else
                {
                    return new Structure((Atom)t);
                }
            } catch(Exception e)
            {
                //logger.log(Level.SEVERE,"Error parsing structure " + sTerm,e);
                return null;
            }
        }

        protected override int? CalcHashCode()
        {
            int? result = base.CalcHashCode();
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
                if(oAsStructure.IsVar())
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
                for(int i = 0; i < ts; i++)
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

        public override int CompareTo(Term t)
        {
            int c = base.CompareTo(t);
            if (c != 0)
            {
                return c;
            }
            if(t.IsStructure())
            {
                Structure s = (Structure)t;
                int ma = GetArity();
                int oa = s.GetArity();
                for (int i = 0; i < ma && i < oa; i++)
                {
                    c = GetTerm(i).CompareTo(s.GetTerm(i));
                    if ( c != 0)
                    {
                        return c;
                    }
                }
            }
            return c;
        }

        public override bool Subsumes(Term t)
        {
            if (t.IsStructure())
            {
                Structure s = (Structure)t;
                int ma = GetArity();
                int oa = s.GetArity();
                for(int i = 0; i < ma && i < oa; i++)
                {
                    if (!GetTerm(i).Subsumes(s.GetTerm(i)){
                        return false;
                    }
                }
                return true;
            } else
            {
                return base.Subsumes(t);
            }
        }

        public override Term Capply(Unifier u) //I dont understand why theres an error here yay 
        {
            return new Structure(this, u);
        }

        public Term Clone() //i dont know if this is override or not yay again
        {
            Structure s = new Structure(this);
            s.hashCodeCache = this.hashCodeCache;
            return s;
        }

        public override Literal CloneNS(Atom newNamespace)
        {
            return new Structure(newNamespace, this);
        }

        public override void AddTerm(Term t)
        {
            if (t == null)
            {
                return;
            }
            if (terms == null)
            {
                terms = new List<Term>(5); //Why??
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


    }
}
