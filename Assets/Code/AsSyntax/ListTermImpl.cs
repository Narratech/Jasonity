using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Assets.Code.ReasoningCycle;
using UnityEngine;

namespace Assets.Code.AsSyntax
{
    public class ListTermImpl : Structure, IListTerm
    {
        private static readonly long serialVersionUID = 1L;

        public static readonly string LIST_FUNCTOR = ".";
        private ITerm term;
        private ITerm next;

        public ListTermImpl() : base(LIST_FUNCTOR, 0)
        {

        }

        public ListTermImpl(ITerm t, ITerm n) : base(LIST_FUNCTOR, 0)
        {
            term = t;
            next = n;
        }

        public static IListTerm ParseList(string sList)
        {
            parser as2j p = new as2j(new StringReader(sList));
            try
            {
                return p.list();
            }
            catch (Exception e)
            {
                Debug.Log(e + ": Error parsing list " + sList);
                return null;
            }
        }

        public IListTerm Append(ITerm t)
        {
            if (IsEmpty())
            {
                term = t;
                next = new ListTermImpl();
                return this;
            }
            else if (IsTail())
            {
                // What to do?
                return null;
            }
            else
            {
                return GetNext().Append(t);
            }
        }

        public ITerm Capply(Unifier u)
        {
            ListTermImpl t = new ListTermImpl();
            if (term != null) t.term = this.term.Capply(u);
            if (next != null) t.next = this.next.Capply(u);
            return t;
        }

        public IListTerm Clone()
        {
            ListTermImpl t = new ListTermImpl();
            if (term != null) t.term = this.term.Clone();
            if (next != null) t.next = this.next.Clone();
            t.hashCodeCache = this.hashCodeCache;
            return t;
        }

        public IListTerm CloneLT()
        {
            return Clone();
        }

        public IListTerm CloneLTShallow()
        {
            ListTermImpl t = new ListTermImpl();
            if (term != null) t.term = this.term;
            if (next != null) t.next = this.next.Clone();
            return t;
        }

        public override bool Equals(object t)
        {
            if (t == null) return false;
            if (t == this) return true;

            if (t.GetType() == typeof(ITerm) && ((ITerm)t).IsVar() ) return false; // unground var is not equals a list
            if (t.GetType() == typeof(IListTerm)) {
                IListTerm tAsList = (IListTerm)t;
                if (term == null && tAsList.GetTerm() != null) return false;
                if (term != null && !term.Equals(tAsList.GetTerm())) return false;
                if (next == null && tAsList.GetNext() != null) return false;
                if (next != null) return next.Equals(tAsList.GetNext());
                return true;
            }
            return false;
        }
        
        public int? CalcHasHCode()
        {
            int code = 37;
            if (term != null) code += term.GetHashCode();
            if (next != null) code += next.GetHashCode();
            return code;
        }

        public ITerm CloneNS(Atom Newnamespace)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object o)
        {
            if (o.GetType() == typeof(VarTerm))
                return o.CompareTo(this) * -1;
            if ((o.GetType() == typeof(INumberTerm))
                return 1;
            if (o.GetType() == typeof(IStringTerm))
                return 1;
            return base.CompareTo(o);
        }

        public int GetArity()
        {
            if (IsEmpty())
                return 0;
            else
                return 2;
        }

        public IListTerm Concat(IListTerm lt)
        {
            if (IsEmpty())
            {
                SetValuesFrom(lt);
            }
            else if (((IListTerm)next).IsEmpty())
            {
                next = lt;
            }
            else
            {
                ((IListTerm)next).Concat(lt);
            }
            return lt.GetLast();
        }

        public void CountVars(Dictionary<VarTerm, int?> c)
        {
            throw new NotImplementedException();
        }

        public IListTerm Difference(IListTerm lt)
        {
            ISet<ITerm> set = new SortedSet<ITerm>();
            set.Add(this);
            set.Remove(lt);
            return SetToList(set);
        }

        public List<ITerm> GetAsList()
        {
            List<ITerm> l = new List<ITerm>();
            foreach (ITerm t in this)
                l.Add(t);
            return l;
        }

        public VarTerm GetCyclicVar()
        {
            throw new NotImplementedException();
        }

        public IListTerm GetLast()
        {
            IListTerm r = this;
            while (!r.IsEnd() && r.GetNext() != null)
                r = r.GetNext();
            return r;
        }

        public IListTerm GetNext()
        {
            if (next.GetType() == typeof(IListTerm))
                return next as IListTerm;
            else
                return null;
        }

        public IListTerm GetPenultimate()
        {
            if (GetNext() == null)
                return null;
            if (IsTail())
                return this;
            if (GetNext().IsEnd() && !GetNext().IsTail())
                return this;
            return GetNext().GetPenultimate();
        }

        public SourceInfo GetSrcInfo()
        {
            throw new NotImplementedException();
        }

        public VarTerm GetTail()
        {
            if (IsTail())
            {
                return (VarTerm)next;
            }
            else if (next != null)
            {
                return GetNext().GetTail();
            }
            else
            {
                return null;
            }
        }

        public ITerm GetTerm()
        {
            return term;
        }

        public ITerm GetTerm(int i)
        {
            if (i == 0) return term;
            if (i == 1) return next;
            return null;
        }

        public List<ITerm> GetTerms()
        {
            Debug.Log("Do not use GetTerms in lists!");
            List<ITerm> l = new List<ITerm>(2);
            if (term != null) l.Add(term);
            if (next != null) l.Add(next);
            return l;
        }

        public void AddTerm(ITerm t)
        {
            Debug.Log("Do not use AddTerm in lists! Use add(Term)");
        }

        public int Size()
        {
            if (IsEmpty())
                return 0;
            else if (IsTail())
                return 1;
            else
                return GetNext().Size() + 1;
        }

        public bool HasVar(VarTerm t, Unifier u)
        {
            throw new NotImplementedException();
        }

        public IListTerm Insert(ITerm t)
        {
            IListTerm n = new ListTermImpl(term, next);
            this.term = t;
            this.next = n;
            return n;
        }

        public bool IsArithExpr()
        {
            throw new NotImplementedException();
        }

        public bool IsAtom()
        {
            return false;
        }

        public bool IsCyclicTerm()
        {
            throw new NotImplementedException();
        }

        public bool IsEnd()
        {
            return IsEmpty() || IsTail();
        }

        public bool IsGround()
        {
            if (IsEmpty())
                return true;
            else if (IsTail())
                return false;
            else if (term != null && term.IsGround())
                return GetNext().IsGround();
            return false;
        }

        public IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
        {
            Debug.Log("ListTermImpl cannot be used for logical consequence!");
            return LogExpr.Empty_Unif_List.Iterator();
        }

        public bool IsInternalAction()
        {
            throw new NotImplementedException();
        }

        public bool IsList()
        {
            return true;
        }

        public bool IsLiteral()
        {
            return false;
        }

        public bool IsEmpty()
        {
            return term == null;
        }

        public bool IsNumeric()
        {
            throw new NotImplementedException();
        }

        public bool IsPlanBody()
        {
            throw new NotImplementedException();
        }

        public bool IsPred()
        {
            throw new NotImplementedException();
        }

        public bool IsRule()
        {
            throw new NotImplementedException();
        }

        public bool IsString()
        {
            throw new NotImplementedException();
        }

        public bool IsStructure()
        {
            throw new NotImplementedException();
        }

        public bool IsTail()
        {
            return next != null && next.IsVar();
        }

        public bool IsUnnamedVar()
        {
            throw new NotImplementedException();
        }

        public bool IsVar()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IListTerm> ListTermIterator()
        {
            
        }

        public ITerm RemoveLast()
        {
            IListTerm p = GetPenultimate();
            if (p != null)
            {
                ITerm b = p.GetTerm();
                p.SetTerm(null);
                p.SetNext(null);
                return b;
            }
            else
            {
                return null;
            }
        }

        public IListTerm Reverse()
        {
            return Reverse_Internal(new ListTermImpl());
        }

        public IListTerm Reverse_Internal(IListTerm r)
        {
            if (IsEmpty())
            {
                return r;
            }
            else if (IsTail())
            {
                r = new ListTermImpl(term.Clone(), r);
                r.SetTail((VarTerm)next.Clone());
                return r;
            }
            else
            {
                return ((ListTermImpl)next).Reverse_Internal(new ListTermImpl(term.Clone(), r));
            }
        }

        public void SetNext(ITerm l)
        {
            next = l;
        }

        public void SetSrcInfo(SourceInfo s)
        {
            throw new NotImplementedException();
        }

        public void SetTail(VarTerm v)
        {
            if (GetNext().IsEmpty())
                next = v;
            else
                GetNext().SetTail(v);
        }

        public void SetTerm(ITerm t)
        {
            term = t;
        }

        public void SetTerm(int i, ITerm t)
        {
            if (i == 0) term = t;
            if (i == 1) next = t;
        }

        public IEnumerator<List<ITerm>> SubSets(int k)
        {
            
        }

        public bool Subsumes(ITerm l)
        {
            throw new NotImplementedException();
        }

        public IListTerm Union(IListTerm lt)
        {
            ISet<ITerm> set = new SortedSet<ITerm>();
            set.Add(lt);
            set.Add(this);
            return SetToList(set);
        }

        public IListTerm Intersection(IListTerm lt)
        {
            ISet<ITerm> set = new SortedSet<ITerm>();
            set.Add(lt);
            set.RetainAll();
            return SetToList(set);
        }

        private IListTerm SetToList(ISet<ITerm> set)
        {
            IListTerm result = new ListTermImpl();
            IListTerm tail = result;
            foreach (ITerm t in set)
            {
                tail = tail.Append(t.Clone());
            }
            return result;
        }

        public IEnumerator<ITerm> Iterator()
        {

        }

        private abstract class ListTermIterator<T> : IEnumerator<T>
        {
            Esto está mal
            public T Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("[");
            IListTerm l = this;
            while (!l.IsEmpty())
            {
                s.Append(l.GetTerm());
                if (l.IsTail())
                {
                    s.Append('|');
                    s.Append(l.GetTail());
                    break;
                }
                l = l.GetNext();
                if (l == null)
                    break;
                if (!l.IsEmpty())
                    s.Append('s');
            }
            s.Append(']');
            return s.ToString();
        }

        public void Add(int index, ITerm o)
        {
            if (index == 0)
                Insert(o);
            else if (index > 0 && GetNext() != null)
                GetNext().Add(index - 1, o);
        }

        public bool Add(ITerm o)
        {
            return GetLast().Append(o) != null;
        }

        public bool AddAll(IList c)
        {
            if (c == null) return false;
            IListTerm lt = this;
            IEnumerator<ITerm> i = c.GetEnumerator();
            Acabar
        }

        public bool AddAll(int index, )
        {

        }

        public void Clear()
        {
            term = null;
            next = null;
        }

        public bool Contains(object o)
        {
            if (term != null && term.Equals(0))
            {
                return true;
            }
            else if (GetNext() != null)
            {
                return GetNext().Contains();
            }
            return false;
        }

        public bool ContainsAll(Collection )
        {

        }

        public ITerm Get(int index)
        {
            if (index == 0)
            {
                return this.term;
            }
            else if (GetNext() != null)
            {
                return GetNext().Get(index - 1);
            }
            return null;
        }

        public int IndexOf(object o)
        {
            if (this.term.Equals(o))
            {
                return 0;
            }
            else if (GetNext() != null)
            {
                int n = GetNext().IndexOf(o);
                if (n >= 0)
                {
                    return n + 1;
                }
            }
            return -1;
        }

        public int LastIndexOf(object arg0)
        {
            return GetAsList().LastIndexOf(arg0);
        }

        public ListIterator<ITerm> listIterator()
        {
            return lsitIterator(0);
        }

        public ListTermIterator<ITerm> listIterator(int startIndex)
        {

        }

        protected void SetValuesFrom(IListTerm lt)
        {
            term = lt.GetTerm();
            next = lt.GetNext();
        }

        public ITerm Remove(int index)
        {
            if(index == 0)
            {
                ITerm bt = this.term;
                if (GetNext() != null)
                {
                    SetValuesFrom(GetNext());
                }
                else
                {
                    Clear();
                }
            }
            else if (GetNext() != null)
            {
                return GetNext().Remove(index - 1);
            }
            return null;
        }

        public bool Remove(object o)
        {
            if (term != null && term.Equals(o))
            {
                if (GetNext() != null)
                {
                    SetValuesFrom(GetNext());
                }
                else
                {
                    Clear();
                }
                return true;
            }
            else if (GetNext() != null)
            {
                return GetNext().Remove(o);
            }
            return false;
        }

        public bool RemoveAll(Collection c)
        {

        }

        public bool RetainAll(Collection c)
        {

        }

        public ITerm Set(int index, ITerm t)
        {
            if (index == 0)
            {
                this.term = (ITerm)t;
                return t;
            }
            else if (GetNext() != null)
            {
                return GetNext().Set(index - 1, t);
            }
            return null;
        }

        public List<ITerm> SubList(int arg0, int arg1)
        {
            return GetAsList().SubList(arg0, arg1);
        }

        public object[] ToArray()
        {
            return ToArray(new object[0]);
        }

        public <T>

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        ITerm ITerm.Clone()
        {
            throw new NotImplementedException();
        }

        public void Add(Pred pred)
        {
            throw new NotImplementedException();
        }

        public void Add(ILogicalFormula logicalFormula)
        {
            throw new NotImplementedException();
        }

        public void Add(Trigger trigger)
        {
            throw new NotImplementedException();
        }

        public void Add(IPlanBody planBody)
        {
            throw new NotImplementedException();
        }
    }
}
