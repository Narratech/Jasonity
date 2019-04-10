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

        public override ITerm Capply(Unifier u)
        {
            ListTermImpl t = new ListTermImpl();
            if (term != null) t.term = this.term.Capply(u);
            if (next != null) t.next = this.next.Capply(u);
            return t;
        }

        public new IListTerm Clone()
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

            if (t.GetType() == typeof(ITerm) && ((ITerm)t).IsVar()) return false; // unground var is not equals a list
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

        public override int CalcHasHCode()
        {
            int code = 37;
            if (term != null) code += term.GetHashCode();
            if (next != null) code += next.GetHashCode();
            return code;
        }

        public override int CompareTo(ITerm o)
        {
            if (o.GetType() == typeof(VarTerm))
                return o.CompareTo(this) * -1;
            if ((o.GetType() == typeof(INumberTerm)))
                return 1;
            if (o.GetType() == typeof(IStringTerm))
                return 1;
            return base.CompareTo(o);
        }

        public override int GetArity()
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

        public override ITerm GetTerm(int i)
        {
            if (i == 0) return term;
            if (i == 1) return next;
            return null;
        }

        public new List<ITerm> GetTerms()
        {
            Debug.Log("Do not use GetTerms in lists!");
            List<ITerm> l = new List<ITerm>(2);
            if (term != null) l.Add(term);
            if (next != null) l.Add(next);
            return l;
        }

        public new void AddTerm(ITerm t)
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
                return GetNext().Count + 1;
        }

        public IListTerm Insert(ITerm t)
        {
            IListTerm n = new ListTermImpl(term, next);
            this.term = t;
            this.next = n;
            return n;
        }

        public override bool IsAtom()
        {
            return false;
        }

        public bool IsEnd()
        {
            return IsEmpty() || IsTail();
        }

        public new bool IsGround()
        {
            if (IsEmpty())
                return true;
            else if (IsTail())
                return false;
            else if (term != null && term.IsGround())
                return GetNext().IsGround();
            return false;
        }

        public override IEnumerator<Unifier> LogicalConsequence(Agent.Agent ag, Unifier un)
        {
            Debug.Log("ListTermImpl cannot be used for logical consequence!");
            return LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
        }


        public override bool IsList()
        {
            return true;
        }

        public override bool IsLiteral()
        {
            return false;
        }

        public bool IsEmpty()
        {
            return term == null;
        }



        public bool IsTail()
        {
            return next != null && next.IsVar();
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
            return ReverseInternal(new ListTermImpl());
        }

        public IListTerm ReverseInternal(IListTerm r)
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
                return ((ListTermImpl)next).ReverseInternal(new ListTermImpl(term.Clone(), r));
            }
        }

        public void SetNext(ITerm l)
        {
            next = l;
        }



        public void SetTail(VarTerm v)
        {
            if (GetNext().Count == 0)
                next = v;
            else
                GetNext().SetTail(v);
        }

        public void SetTerm(ITerm t)
        {
            term = t;
        }

        public override void SetTerm(int i, ITerm t)
        {
            if (i == 0) term = t;
            if (i == 1) next = t;
        }

        public IEnumerator<List<ITerm>> SubSets(int k)
        {
            Acabaaaar
        }


        /*PIFOSTIO EL QUE SE VIENE*/




        /**************************/

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
            set.RetainAll(this);
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



        public IEnumerator<IListTerm> ListTermIterator()
        {
            return new ListTermIterator<IListTerm>(this);
        }

        public class ListTermIterator<IListTerm>
        {
            public ListTermIterator()
            {

            }

            public IListTerm Next()
            {
                MoveNext();
                return current;
            }
        }


        public IEnumerator<ITerm> Iterator()
        {

        }

        private abstract class ListTermIterator<T> : IEnumerator<T>
        {

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
            while (!(l.Count == 0))
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
                if (!(l.Count == 0))
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
                GetNext().Insert(index - 1, o);
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
            while (i.MoveNext())
            {
                lt = lt.Append(i.Next());
            }
            return true;
        }

        public bool AddAll(int index, IList c)
        {
            IEnumerator<ITerm> i = c.GetEnumerator();
            int p = index;
            while (i.MoveNext())
            {
                Add(p, i.Next());
                p++;
            }
            return true;
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
                return GetNext().Contains(o);
            }
            return false;
        }

        public bool ContainsAll(IList c)
        {
            bool r = true;
            IEnumerator<ITerm> i = c.GetEnumerator();
            while (i.MoveNext() && r)
            {
                r = r && Contains(i.Next());
            }
            return r;
        }

        public ITerm Get(int index)
        {
            if (index == 0)
            {
                return this.term;
            }
            else if (GetNext() != null)
            {
                return GetNext()[index - 1];
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

        public IEnumerator<ITerm> listIterator()
        {
            return listIterator(0);
        }

        public IEnumerator<ITerm> listIterator(int startIndex)
        {
            ListTermImpl list = this;
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
                ITerm aux = GetNext()[index-1];
                GetNext().RemoveAt(index - 1);
                return aux;
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
                return GetNext().Remove((ITerm)o);
            }
            return false;
        }

        public bool RemoveAll(IList c)
        {
            bool r = true;
            IEnumerator i = c.GetEnumerator();
            while (i.MoveNext() && r)
            {
                r = r && Remove(i.Current);
            }
            return r;
        }

        public bool RetainAll(IList c)
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
                ITerm aux = GetNext()[index - 1];
                GetNext().Insert(index -1, t);
                return aux;
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

        public <T> T[] ToArray(T[] a)
        {
            
        }

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

        public ITerm CloneNS(Atom Newnamespace)
        {
            throw new NotImplementedException();
        }

        public VarTerm GetCyclicVar()
        {
            throw new NotImplementedException();
        }

        public void CountVars(Dictionary<VarTerm, int?> c)
        {
            throw new NotImplementedException();
        }

        public bool IsUnnamedVar()
        {
            throw new NotImplementedException();
        }

        public bool IsVar()
        {
            throw new NotImplementedException();
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

        public bool Subsumes(ITerm l)
        {
            throw new NotImplementedException();
        }

        public SourceInfo GetSrcInfo()
        {
            throw new NotImplementedException();
        }

        public bool IsInternalAction()
        {
            throw new NotImplementedException();
        }

        public bool HasVar(VarTerm t, Unifier u)
        {
            throw new NotImplementedException();
        }

        public bool IsArithExpr()
        {
            throw new NotImplementedException();
        }

        public void SetSrcInfo(SourceInfo s)
        {
            throw new NotImplementedException();
        }

        public bool IsCyclicTerm()
        {
            throw new NotImplementedException();
        }
    }
}
