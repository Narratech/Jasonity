using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Code.AsSyntax
{
    /*
    * Represents a variable Term: like X (starts with upper case)
    */
    public class VarTerm: LiteralImpl, INumberTerm, IListTerm
    {
        public int Count { get { return 1;  }  } // Lo hemos implementado nosotros, no creemos que necesite un set.... y creemos que siempre va a ser 1 por ser una variable
            
        public bool IsReadOnly { get; set; }  // Lo hemos implementado nosotros, suponemos que puede tener sus get y sus set

        public ITerm this[int index] { get { if (index == 0) return this.GetTerm(); else return null; } set {; } }

        public VarTerm(string s):base(s)
        {
            if (s != null && char.IsLower(s, 0))
            {
                Exception e = new Exception("stack");
                //e.printStackTrace();
            }
        }

        public VarTerm(Atom @namespace, string functor):
            base(@namespace, LPos, functor)
        {
            
        }

        public VarTerm(Atom @namespace, Literal v): base(@namespace, !v.Negated(), v)
        {
            
        }

        public override ITerm Capply(Unifier u)
        {
            if (u != null)
            {
                ITerm vl = u.Get(this);
                if (vl != null)
                {
                    if (!vl.IsCyclicTerm() && vl.HasVar(this, u))
                    {
                        u.Remove(this);
                        ITerm tempVl = vl.Capply(u);
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
                        vl = ((Literal)vl).ForceFullLiteralImpl().AddAnnots((IListTerm)this.GetAnnots().Capply(u));
                    }
                    return vl;
                }
            }
            return Clone();
        }

        public override ITerm Clone()
        {
            return new VarTerm(this.GetNS(), this);
        }

        public virtual new Literal CloneNS(Atom newNamespace)
        {
            return new VarTerm(newNamespace, this);
        }

        public IListTerm CloneLT()
        {
            return Clone() as IListTerm;
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

        public override int CalcHashCode()
        {
            int result = GetFunctor().GetHashCode();
            return result;
        }

        public override int CompareTo(ITerm t)
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

        public override bool Subsumes(ITerm t)
        {
            return true;
        }

        public override IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
        {
            ITerm t = this.Capply(un);
            if (t.Equals(this))
            {
                return base.LogicalConsequence(ag, un);
            }
            else
            {
                return ((ILogicalFormula)t).LogicalConsequence(ag, un);
            }
        }

        public override ITerm GetTerm(int i)
        {
            return null;
        }

        public override void AddTerm(ITerm t)
        {

        }

        
        public override int GetArity()
        {
            return 0;
        }

        public override List<ITerm> GetTerms()
        {
            return null;
        }

        public override Literal SetTerms(List<ITerm> l)
        {
            return this;
        }

        public override void SetTerm(int i, ITerm t)
        {

        }

        public override Literal AddTerms(List<ITerm> l)
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
                ITerm vl = u.Get(this);
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

        public void Add(int index, ITerm o) { }

        public bool Add(ITerm o)
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

        public ITerm Get(int index)
        {
            return null;
        }

        public int IndexOf(object o)
        {
            return -1;
        }

        public int LastIndexOf(object o)
        {
            return -1;
        }

        // En Jason devuelven null... pero parece más correcto devolver
        public IEnumerator<ITerm> GetEnumerator()
        {
            // Dicen que lo elegante es hacer un foreach y devolver todos los items de la lista... pero como yo creo que sólo hay uno
            //foreach (Item item in _itemList) {
            //    yield return item;
            //}
            yield return this[0]; // Fede se ha flipado con esto
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // Dicen que lo elegante es llamar al otro método
        }

        public IEnumerator<ITerm> ListEnumerator()
        {
            return null;
        }

        public IEnumerator<ITerm> ListEnumerator(int index)
        {
            return null;
        }
        

        public ITerm Remove(int index)
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

        public ITerm Set(int index, ITerm o)
        {
            return null;
        }

        public List<ITerm> SubList(int arg0, int arg1)
        {
            return null;
        }

        public IEnumerator<List<ITerm>> SubSets(int k)
        {
            return null;
        }

        public object[] ToArray(object[] arg0)
        {
            return null;
        }

        public void SetTerm(ITerm t) { }

        public void SetNext(ITerm t) { }

        public IListTerm Append(ITerm t) { return null; }

        public IListTerm Insert(ITerm t) { return null; }

        public IListTerm Concat(IListTerm lt) { return null; }

        public IListTerm Reverse() { return null; }

        public IListTerm Union(IListTerm lt) { return null; }

        public IListTerm Intersection(IListTerm lt) { return null; }

        public IListTerm Difference(IListTerm lt) { return null; }

        public List<ITerm> GetAsList() { return null; }

        public IListTerm GetLast() { return null; }

        public IListTerm GetPenultimate() { return null; }

        public ITerm RemoveLast() { return null; }

        public IListTerm GetNext() { return null; }

        public ITerm GetTerm() { return null; }

        public bool IsEmpty() { return false; }

        public bool IsEnd() { return false; }

        public bool IsTail() { return false; }

        public void SetTail(VarTerm v) { }

        public VarTerm GetTail() { return null; }

        public IEnumerator<IListTerm> ListTermIterator() { return null; }

        public int Size() { return -1; }

        public IListTerm CloneLTShallow() { return null; }

        ITerm ITerm.CloneNS(Atom Newnamespace)
        {
            throw new NotImplementedException();
        }

        public override void SetSrcInfo(SourceInfo s)
        {
            base.SetSrcInfo(s);
        }

        public override SourceInfo GetSrcInfo()
        {
            return base.GetSrcInfo();
        }



        public new int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(ITerm item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, ITerm item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void ICollection<ITerm>.Add(ITerm item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ITerm item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ITerm[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ITerm item)
        {
            return default; //null;
        }
    }
}
