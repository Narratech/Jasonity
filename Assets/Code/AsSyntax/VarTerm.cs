using Assets.Code.Logic.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    /*
    * Represents a variable Term: like X (starts with upper case)
    */
    public class VarTerm: LiteralImpl, NumberTerm, ListTerm
    {
        private static readonly long serialVersionUID = 1L;
        
        public VarTerm(string s):base(s)
        {
            if (s != null && Char.IsLower(s, 0))
            {
                Exception e = new Exception("stack");
                //e.printStackTrace();
            }
        }

        public VarTerm(Atom @namespace, string functor):
            base(@namespace, LPos/*PERO QUÉ HOOOOOSTIAS ES ESTO?!*/, functor)
        {
            
        }

        public VarTerm(Atom @namespace, Literal v): base(@namespace, !v.Negated(), v)
        {
            
        }

        public override Term Capply(Unifier u)
        {
            if (u != null)
            {
                Term vl = u.Get(this);
                if (vl != null)
                {
                    if (!vl.IsCyclicTerm() && vl.HasVar(this, u))
                    {
                        u.Remove(this);
                        Term tempVl = vl.Capply(u);
                        u.Bind(this, vl);

                        CyclicTerm ct = new CyclicTerm(tempVl as Literal, this);
                        Unifier renamedVars = new Unifier();
                        ct.MakeVarsAnnon(renamedVars);
                        renamedVars.Remove(this);
                        u.Compose(renamedVars);
                        vl = ct;
                    }

                    vl = vl.Capply(u);

                    if (vl.IsLiteral())
                    {
                        if (GetNS() != Literal.DefaultNS)
                        {
                            vl = (vl.CloneNS(GetNS().Capply(u) as Atom) as Literal);
                        }
                        if (Negated())
                        {
                            ((Literal)vl).SetNegated(Literal.LNeg);
                        }
                    }

                    if (vl.IsLiteral() && this.HasAnnot())
                    {
                        vl = ((Literal)vl).ForceFullLiteralImpl().AddAnnots((ListTerm)this.GetAnnots().Capply(u));
                    }
                    return vl;
                }
            }
            return Clone();
        }

        public override Term Clone()
        {
            return new VarTerm(this.GetNS(), this);
        }

        public override Literal CloneNS(Atom newNamespace)
        {
            return new VarTerm(newNamespace, this);
        }

        public ListTerm CloneLT()
        {
            return Clone() as ListTerm;
        }

        public override bool IsVar()
        {
            return true;
        }

        public override bool IsUnnamedVar()
        {
            return false;
        }

        public override bool IsGround()
        {
            return false;
        }

        public override bool Equals(object t)
        {
            if (t == null) return false;
            if (t == this) return true;

            if (t.GetType() == typeof(VarTerm))
            {
                VarTerm tAsVT = t as VarTerm; //This should be const but c# doesn't allow it
                return GetFunctor().Equals(tAsVT.GetFunctor());
            }
            return false;
        }

        public override int? CalcHashCode()
        {
            int result = GetFunctor().GetHashCode();
            return result;
        }

        public override int CompareTo(Term t)
        {
            if (t == null || t.IsUnnamedVar())
            {
                return -1;
            }
            else if (t.IsVar())
            {
                return GetFunctor().CompareTo(((VarTerm)t).GetFunctor());
            }
            else
            {
                return 1;
            }
        }

        public override bool Subsumes(Term t)
        {
            return true;
        }

        public override IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
        {
            Term t = this.Capply(un);
            if (t.Equals(this))
            {
                return base.LogicalConsequence(ag, un);
            }
            else
            {
                return ((LogicalFormula)t).LogicalConsequence(ag, un);
            }
        }

        public override Term GetTerm(int i)
        {
            return null;
        }

        public override void AddTerm(Term t)
        {

        }

        
        public override int GetArity()
        {
            return 0;
        }

        public override List<Term> GetTerms()
        {
            return null;
        }

        public override Literal SetTerms(List<Term> l)
        {
            return this;
        }

        public override void SetTerm(int i, Term t)
        {

        }

        public override Literal AddTerms(List<Term> l)
        {
            return this;
        }

        public override bool IsLiteral()
        {
            return false;
        }

        public override bool IsRule()
        {
            return false;
        }

        public override bool IsList()
        {
            return false;
        }

        public override bool IsString()
        {
            return false;
        }

        public override bool IsInternalAction()
        {
            return false;
        }

        public override bool IsArithExpr()
        {
            return false;
        }

        public override bool IsNumeric()
        {
            return false;
        }

        public override bool IsPred()
        {
            return false;
        }

        public override bool IsStructure()
        {
            return false;
        }

        public override bool IsAtom()
        {
            return false;
        }

        public override bool IsPlanBody()
        {
            return false;
        }

        public override bool IsCyclicTerm()
        {
            return false;
        }

        public override bool HasVar(VarTerm t, Unifier u)
        {
            if (Equals(t))
            {
                return true;
            }
            if (u != null)
            {
                Term vl = u.Get(this);
                if (vl != null)
                {
                    try
                    {
                        u.Remove(this);
                        return vl.HasVar(t, u);
                    }
                    finally
                    {
                        u.Bind(this, vl);
                    }
                }
            }
            return false;
        }

        public override VarTerm GetCyclicVar()
        {
            throw new NotImplementedException();
        }

        public override void CountVars(Dictionary<VarTerm, int?> c)
        {
            int? n = c.ContainsKey(this) ? c[this] : 0;
            c.Add(this, n+1);
            base.CountVars(c);
        }

        public override bool CanBeAddedInBB()
        {
            return false;
        }

        public double Solve()
        {
            throw new Exception();
        }

        public void Add(int index, Term o) { }

        public bool Add(Term o)
        {
            return false;
        }

        public bool AddAll()
        {
            return false;
        }

        public bool AddAll(int index)
        {
            return false;
        }

        public void Clear() { }

        public bool Contains(object o)
        {
            return false;
        }

        public bool ContainsAll()
        {
            return false;
        }

        public Term Get(int index)
        {
            return null;
        }

        public int IndexOf(object o)
        {
            return -1;
        }

        public int lastIndexOf(object o)
        {
            return -1;
        }

        public IEnumerator<Term> Iterator()
        {
            return null;
        }

        /*
        public ListIterator<Term> ListIterator()
        {
            return null;
        }

        public ListIterator<Term> ListIterator(int index)
        {
            return null;
        }
        */

        public Term Remove(int index)
        {
            return null;
        }

        public bool Remove(object o)
        {
            return false;
        }

        public bool RemoveAll()
        {
            return false;
        }

        public bool RetainAll()
        {
            return false;
        }

        public Term Set(int index, Term o)
        {
            return null;
        }

        public List<Term> SubList(int arg0, int arg1)
        {
            return null;
        }

        public IEnumerator<List<Term>> SubSets(int k)
        {
            return null;
        }

        public object[] ToArray(object[] arg0)
        {
            return null;
        }

        public void SetTerm(Term t) { }

        public void SetNext(Term t) { }

        public ListTerm Append(Term t) { return null; }

        public ListTerm Insert(Term t) { return null; }

        public ListTerm Concat(ListTerm lt) { return null; }

        public ListTerm Reverse() { return null; }

        public ListTerm Union(ListTerm lt) { return null; }

        public ListTerm Intersection(ListTerm lt) { return null; }

        public ListTerm Difference(ListTerm lt) { return null; }

        public List<Term> GetAsList() { return null; }

        public ListTerm GetLast() { return null; }

        public ListTerm GetPenultimate() { return null; }

        public Term RemoveLast() { return null; }

        public ListTerm GetNext() { return null; }

        public Term GetTerm() { return null; }

        public bool IsEmpty() { return false; }

        public bool IsEnd() { return false; }

        public bool IsTail() { return false; }

        public void SetTail(VarTerm v) { }

        public VarTerm GetTail() { return null; }

        public IEnumerator<ListTerm> ListTermIterator() { return null; }

        public int Size() { return -1; }

        public ListTerm CloneLTShallow() { return null; }

        Term Term.CloneNS(Atom Newnamespace)
        {
            throw new NotImplementedException();
        }

        public override void SetSrcInfo(SourceInfo s)
        {
            throw new NotImplementedException();
        }

        public override SourceInfo GetSrcInfo()
        {
            throw new NotImplementedException();
        }

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
