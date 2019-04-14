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

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public ITerm this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ListTermImpl() : base(LIST_FUNCTOR, 0)
        {

        }

        public ListTermImpl(ITerm t, ITerm n) : base(LIST_FUNCTOR, 0)
        {
            term = t;
            next = n;
        }

        /**
         * Adds a term in the end of the list
         * @return the ListTerm where the term was added (i.e. the last ListTerm of the list)
         */
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
                return null;
            }
            else
            {
                return GetNext().Append(t);
            }
        }

        /** make a hard copy of the terms */
        public override ITerm Capply(Unifier u)
        {
            ListTermImpl t = new ListTermImpl();
            if (term != null) t.term = this.term.Capply(u);
            if (next != null) t.next = this.next.Capply(u);
            return t;
        }

        /** make a hard copy of the terms */
        public new IListTerm Clone()
        {
            ListTermImpl t = new ListTermImpl();
            if (term != null) t.term = this.term.Clone();
            if (next != null) t.next = this.next.Clone();
            t.hashCodeCache = this.hashCodeCache;
            return t;
        }

        /** make a hard copy of the terms */
        public IListTerm CloneLT()
        {
            return Clone();
        }

        /** make a shallow copy of the list (terms are not cloned, only the structure) */
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

        public override int CalcHashCode()
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

        // for unifier compatibility
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
            else if (((IListTerm)next).Count == 0)
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

        /**
         * Returns this ListTerm as a Java List (implemented by ArrayList).
         * Note: the tail of the list, if any, is not included! 
         */
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

        // for unifier compatibility
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

        /** return the this ListTerm elements (0=Term, 1=ListTerm) */
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

        /**
         * insert a term in the begin of this list
         * @return the new starter of the list
         */
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

        /**
         * Creates a new (cloned) list with the same elements of this list, but in the reversed order.
         * The Tail remains the Tail: reverse([a,b|T]) = [b,a|T].
         */
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

        /** returns all subsets that take k elements of this list */
        public IEnumerator<List<ITerm>> SubSets(int k)
        {
            return new MyIEnumerator<List<ITerm>>(k);
        }

        private class MyIEnumerator<T> : IEnumerator<T> where T : List<ITerm>
        {
            private int k;

            public MyIEnumerator(int k)
            {
                this.k = k;
            }

            LinkedList<SubSetSearchState> open = null;

            ITerm[] thisAsArray = new ITerm[0];

            List<ITerm> next = null;

            public bool HasNext()
            {
                if (open == null)
                {
                    open = new LinkedList<>();
                    thisAsArray = GetAsList().ToArray(thisAsArray);
                    open.AddAfter(new SubSetSearchState(0, k, null, null));
                }
                if (next == null)
                {
                    GetNext();
                }
                return next != null;
            }

            public List<ITerm> Next()
            {
                if (next == null)
                {
                    GetNext();
                }
                List<ITerm> r = next;
                return r;
            }

            void GetNext()
            {
                while (!(open.Count == 0))
                {
                    SubSetSearchState s = open.RemoveFirst();
                    if (s.d == 0)
                    {
                        next = s.GetAsList();
                        return;
                    }
                    else
                    {
                        s.AddNext();
                    }
                }
                next = null;
            }

            public void Remove() { }

            class SubSetSearchState
            {
                int pos;
                int d;
                ITerm value = null;
                SubSetSearchState f = null;

                SubSetSearchState(int pos, int d, ITerm t, SubSetSearchState father)
                {
                    this.pos = pos;
                    this.d = d;
                    value = t;
                    f = father;
                }

                void AddNexts()
                {
                    int pSize = (k - d) + thisAsArray.Length;
                    for (int i = thisAsArray.Length-1; i>= pos; i--)
                    {
                        if (pSize - i >= k)
                        {
                            open.AddFirst(new SubSetSearchState(i+1, d- 1, thisAsArray[i], this));
                        }
                    }
                }

                List<ITerm> GetAsList()
                {
                    LinkedList<ITerm> np = new LinkedList<>();
                    SubSetSearchState c = this;
                    while (c.value != null)
                    {
                        np.AddFirst(c.value);
                        c = c.f;
                    }
                    return np;
                }
            }

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

        /** returns a new (cloned) list representing the 
         * set resulting of the union of this list and lt. */
        public IListTerm Union(IListTerm lt)
        {
            ISet<ITerm> set = new SortedSet<ITerm>();
            set.Add(lt);
            set.Add(this);
            return SetToList(set);
        }

        /** returns a new (cloned) list representing the 
         * set resulting of the intersection of this list and lt. */
        public IListTerm Intersection(IListTerm lt)
        {
            ISet<ITerm> set = new SortedSet<ITerm>();
            set.Add(lt);
            set.UnionWith(this);
            return SetToList(set);
        }

        // copy the set to a new list
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

        /**
         * gives an iterator that includes the final empty list or tail,
         * for [a,b,c] returns [a,b,c]; [b,c]; [c]; and [].
         * for [a,b|T] returns [a,b|T]; [b|T]; [b|T]; and T.
         */
        public IEnumerator<IListTerm> ListTermIteratorFunc()
        {
            return new MyListTermIterator<IListTerm>(this);
        }

        private class MyListTermIterator<T>: ListTermIterator<T> where T: IListTerm
        {
            public MyListTermIterator(T l) : base(l)
            {

            }

            public IListTerm Next()
            {
                MoveNext();
                return current;
            }
        }

        /**
         * returns an iterator where each element is a Term of this list,
         * the tail of the list is not considered.
         * for [a,b,c] returns 'a', 'b', and 'c'.
         * for [a,b|T] returns 'a' and 'b'.
         */
         //Debería llamarse Enumerator
        public IEnumerator<ITerm> Iterator()
        {
            return new MyListTermIterator2<ITerm>(this);
        }

        private class MyListTermIterator2<T> : ListTermIterator<T> where T: ITerm
        {
            public MyListTermIterator2(T l) : base(l)
            {

            }

            public override bool HasNext()
            {
                return nextLT != null && !(nextLT.Count == 0) && nextLT.IsList();
            }

            public ITerm Next()
            {
                MoveNext();
                return current.GetTerm();
            }
        }

        private abstract class ListTermIterator<T>: IEnumerator<T> where T: ITerm
        {
            public IListTerm nextLT;
            public IListTerm current = null;
            public ListTermIterator(T lt)
            {
                //Probable que explote, trabajar con ITerm
                nextLT = (IListTerm)lt;
            }

            public T Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public ListTermIterator(IListTerm lt)
            {
                nextLT = lt;
            }

            public virtual bool HasNext()
            {
                return nextLT != null;
            }

            public void MoveNext()
            {
                current = nextLT;
                nextLT = nextLT.GetNext();
            }

            public void Remove()
            {
                if (current != null && nextLT != null)
                {
                    current.SetTerm(nextLT.GetTerm());
                    current.SetNext(nextLT.GetNext());
                    nextLT = current;
                }
            }

            bool IEnumerator.MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        
        // TODO: do not base the implementation of listIterator on get (that is O(n))
        // conversely, implement all other methods of List based on this iterator
        // (see AbstractSequentialList)
        // merge code of ListTermIterator here and use always the same iterator
        public IEnumerator<ITerm> listIterator(int startIndex)
        {
            ListTermImpl list = this;
            return new MyListIterator<ITerm>(startIndex, list);
        }

        private class MyListIterator<ITerm>:IEnumerator<ITerm>
        {
            ListTermImpl list;
            int pos, startIndex;
            int last;
            int size;

            public MyListIterator(int startIndex, ListTermImpl list)
            {
                this.list = list;
                pos = startIndex;
                this.startIndex = startIndex;
                last = -1;
                size = Size();
            }

            public ITerm Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public void Add(ITerm o)
            {
                list.Add(last, o);
            }
            public bool HasNext()
            {
                return pos < size;
            }
            public bool HasPrevious()
            {
                return pos > startIndex;
            }
            public ITerm Next()
            {
                last = pos;
                pos++;
                return Get(last);
            }
            public int NextIndex()
            {
                return pos + 1;
            }
            public ITerm Previous()
            {
                last = pos;
                pos--;
                return Get(last);
            }
            public int PreviousIndex()
            {
                return pos - 1;
            }
            public void Remove()
            {
                list.Remove(last);
            }
            public void Set(ITerm o)
            {
                Remove();
                Add(o);
            }

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
            bool r = true;
            IEnumerator i = Iterator();
            while (i.MoveNext())
            {
                ITerm t = (ITerm)i.Current;
                if (!c.Contains(t))
                {
                    r = r && Remove(t);
                }
            }
            return r;
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

        public T[] ToArray<T>(T[] a)
        {
            int s = Size();
            if (a.Length < s)
                a = (T[])java.lang.reflect.Array.newInstance(a.getClass().getComponentType(), s);

            int i = 0;
            foreach (ITerm t in this)
            {
                a[i++] = (T)t;
            }
            if (a.Length > s)
            {
                a[s] = default(T);//null;
            }

            return a;
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

        IEnumerator<IListTerm> IListTerm.ListTermIterator()
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
            throw new NotImplementedException();
        }

        public IEnumerator<ITerm> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
