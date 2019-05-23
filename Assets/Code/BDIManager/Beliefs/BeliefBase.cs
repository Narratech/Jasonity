// Implements a default Belief Base
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;

namespace BDIManager.Beliefs
{
    public class BeliefBase
    {
        private Dictionary<PredicateIndicator, BelEntry> belsMapDefaultNS = new Dictionary<PredicateIndicator, BelEntry>();

        private Dictionary<Atom, Dictionary<PredicateIndicator, BelEntry>> nameSpaces = new Dictionary<Atom, Dictionary<PredicateIndicator, BelEntry>>();

        private int size = 0;

        public HashSet<Literal> percepts = new HashSet<Literal>();

        public static readonly ITerm APercept = new Atom("percept");
        public static readonly ITerm TPercept = Pred.CreateSource(APercept);
        public static readonly ITerm ASelf = new Atom("self");
        public static readonly ITerm TSelf = Pred.CreateSource(ASelf);

        // Constructor
        public BeliefBase()
        {
            nameSpaces.Add(Literal.DefaultNS, belsMapDefaultNS);
        }

        //public void Init(Agent ag)
        //{
        //    if (ag != null) { /* logger */}
        //}

        // Returns the namespaces
        public HashSet<Atom> GetNameSpaces()
        {
            return new HashSet<Atom>(nameSpaces.Keys); // This might be wrong
        }

        // Returns the size
        public int Size()
        {
            return size;
        }

        // Clears the belief base
        public void Clear()
        {
            size = 0;
            percepts.Clear();
            belsMapDefaultNS.Clear();
            nameSpaces.Clear();
            nameSpaces.Add(Literal.DefaultNS, belsMapDefaultNS);
        }

        public IEnumerator<Literal> GetPercepts()
        {
            IEnumerator<Literal> i = percepts.GetEnumerator();
            return new IEnumeratorGetPercepts(i, this);
        }

        // Returns percepts
        public HashSet<Literal> GetPerceptsSet() => percepts;

        // Adds a new literal
        public bool Add(Literal l) => Add(l, false);

        // Adds a new literal in a specific index
        public bool Add(int index, Literal l) => Add(l, index != 0);

        // Adds a new literal at the end of the belief base
        protected bool Add(Literal l, bool addInEnd)
        {
            if (!l.CanBeAddedInBB()) return false;

            Literal bl = Contains(l);
            if (bl != null && !bl.IsRule())
            {
                if (bl.ImportAnnots(l))
                {
                    if (l.HasAnnot(TPercept))
                    {
                        percepts.Add(bl);
                    }
                    return true;
                }
            }
            else
            {
                // new belief
                l = l.Copy();
                BelEntry entry = ProvideBelEntry(l);
                entry.Add(l, addInEnd);

                // Add it to percepts list
                if (l.HasAnnot(TPercept))
                {
                    percepts.Add(l);
                }

                size++;
                return true;
            }
            return false;
        }

        private BelEntry ProvideBelEntry(Literal l)
        {
            Dictionary<PredicateIndicator, BelEntry> belsMap = belsMapDefaultNS;
            if (l.GetNS() != Literal.DefaultNS)
            {
                belsMap = nameSpaces[l.GetNS()];
                if (belsMap == null)
                {
                    belsMap = new Dictionary<PredicateIndicator, BelEntry>();
                    nameSpaces.Add(l.GetNS(), belsMap);
                }
            }
            BelEntry entry = belsMap[l.GetPredicateIndicator()];
            if (entry == null)
            {
                entry = new BelEntry();
                belsMap.Add(l.GetPredicateIndicator(), entry);
            }
            return entry;
        }

        // Removes a literal from the belief base
        public bool Remove(Literal l)
        {
            Literal bl = Contains(l);
            if (bl != null)
            {
                if (l.HasSubsetAnnot(bl))
                {
                    if (l.HasAnnot(TPercept))
                    {
                        percepts.Remove(bl);
                    }
                    bool result = bl.DelAnnots((List<ITerm>)l.GetAnnots());
                    return RemoveFromEntry(bl) || result;
                }
            }
            return false;
        }

        private bool RemoveFromEntry(Literal l)
        {
            if (l.HasSource())
            {
                return false;
            }
            else
            {
                Dictionary<PredicateIndicator, BelEntry> belsMap = l.GetNS() == Literal.DefaultNS ? belsMapDefaultNS : nameSpaces[l.GetNS()];
                PredicateIndicator key = l.GetPredicateIndicator();
                BelEntry entry = belsMap[key];
                entry.Remove(l);
                if (entry.IsEmpty())
                {
                    belsMap.Remove(key);
                }
                size--;
                return true;
            }
        }

        public IEnumerator<Literal> GetEnumerator()
        {
            IEnumerator<Dictionary<PredicateIndicator, BelEntry>> ins = nameSpaces.Values.GetEnumerator();
            return new IEnumeratorEnumerator(ins, percepts, size);
        }

        public bool Abolish(Atom nameSpace, PredicateIndicator pi)
        {
            
            BelEntry entry = nameSpaces[nameSpace][pi];
            nameSpaces[nameSpace].Remove(pi);
            if (entry != null)
            {
                size -= entry.Size();
                IEnumerator<Literal> i = percepts.GetEnumerator();
                while (i.MoveNext())
                {
                    Literal l = i.Current;
                    if (l.GetPredicateIndicator().Equals(pi)) i.Dispose();
                }
                return true;
            }
            else return false;
        }

        // Checks if the belief base contains a specific literal
        public Literal Contains(Literal l)
        {
            Dictionary<PredicateIndicator, BelEntry> belsMap = l.GetNS() == Literal.DefaultNS ? belsMapDefaultNS : nameSpaces[l.GetNS()];
            if (belsMap == null)
            {
                return null;
            }
            BelEntry entry = belsMap[l.GetPredicateIndicator()];
            if (entry == null)
            {
                return null;
            }
            else
            {
                return entry.Contains(l);
            }
        }

        public IEnumerator<Literal> GetCandidateBeliefs(PredicateIndicator pi)
        {
            Dictionary<PredicateIndicator, BelEntry> pi2entry = nameSpaces[pi.GetNS()];
            if (pi2entry == null) return null;

            BelEntry entry = pi2entry[pi];
            if (entry != null) return new EntryIteratorWrapper(entry, percepts, size);
            else return null;
        }

        public IEnumerator<Literal> GetCandidateBeliefs(Literal l, Unifier u)
        {
            if (l.IsVar()) return GetEnumerator();
            else
            {
                Dictionary<PredicateIndicator, BelEntry> belsMap = belsMapDefaultNS;
                if (l.GetNS() != Literal.DefaultNS)
                {
                    Atom ns = l.GetNS();
                    if (ns.IsVar())
                    {
                        l = (Literal)l.CApply(u);
                        ns = l.GetNS();
                    }
                    if (ns.IsVar()) return GetEnumerator();
                    belsMap = nameSpaces[ns];
                }
                if (belsMap == null) return null;
                BelEntry entry = belsMap[l.GetPredicateIndicator()];
                if (entry != null) return new EntryIteratorWrapper(entry, percepts, size);
                else return null;
            }
        }

        public override string ToString() => nameSpaces.ToString();

        public BeliefBase Clone()
        {
            BeliefBase bb = new BeliefBase();
            foreach (Literal b in this) bb.Add(1, b.Copy());
            return bb;
        }

        class EntryIteratorWrapper : IEnumerator<Literal>
        {
            Literal last = null;
            IEnumerator<Literal> il = null;
            BelEntry entry = null;
            HashSet<Literal> percepts;
            int size;

            public EntryIteratorWrapper(BelEntry e, HashSet<Literal> percepts, int size)
            {
                entry = e;
                il = entry.list.GetEnumerator();
                this.percepts = percepts;
                this.size = size;
            }

            public Literal Current => il.Current;

            object IEnumerator.Current => il.Current;

            public void Dispose()
            {
                il.Dispose();
            }

            public bool HasNext() => il.MoveNext();

            public bool MoveNext()
            {
                return il.MoveNext();
            }

            public Literal Next()
            {
                last = il.Current;
                return last;
            }

            public void Remove()
            {
                il.Dispose();
                entry.Remove(last);
                if (last.HasAnnot(TPercept)) percepts.Remove(last);
                size--;
            }

            public void Reset()
            {
                il.Reset();
            }
        }

        // Each predicate indicator has one BelEntry assigned
        class BelEntry
        {
            public List<Literal> list = new List<Literal>(); // Keeps the order of the beliefs
            private Dictionary<StructureWrapperForLiteral, Literal> map = new Dictionary<StructureWrapperForLiteral, Literal>();

            public void Add(Literal l, bool addInEnd)
            {
                map.Add(new StructureWrapperForLiteral(l), l);
                if (addInEnd)
                {
                    list.Add(l);
                }
                else
                {
                    list.Insert(list.Count, l);
                }
            }

            public void Remove(Literal l)
            {
                Literal linmap = map[new StructureWrapperForLiteral(l)];
                map.Remove(new StructureWrapperForLiteral(l));
                if (linmap != null)
                {
                    list.Remove(linmap);
                }
            }

            public int Size()
            {
                return map.Count;
            }

            public bool IsEmpty()
            {
                return list.Count == 0;
            }

            public Literal Contains(Literal l)
            {
                return map[new StructureWrapperForLiteral(l)];
            }

            protected object Clone()
            {
                BelEntry be = new BelEntry();
                foreach (Literal l in list) be.Add(l.Copy(), false);
                return be;
            }

            public override string ToString()
            {
                StringBuilder s = new StringBuilder();
                foreach (Literal l in list) s.Append(l + ":" + l.GetHashCode() + ",");
                return s.ToString();
            }
        }

        private class IEnumeratorGetPercepts : IEnumerator<Literal>
        {
            IEnumerator<Literal> i = null;
            Literal current = null;
            private BeliefBase bb;

            public IEnumeratorGetPercepts(IEnumerator<Literal> i, BeliefBase bb)
            {
                this.i = i;
                this.bb = bb;
            }

            public Literal Current => i.Current;

            object IEnumerator.Current => i.Current;

            public void Dispose()
            {
                i.Dispose();
            }

            public bool HasNext() => i.MoveNext();

            public bool MoveNext()
            {
                return i.MoveNext();
            }

            public Literal Next()
            {
                current = i.Current;
                return current;
            }
            public void Remove()
            {
                if (current == null) Console.WriteLine("No perception to remove!");
                // Remove from percepts
                i.Dispose();
                // Remove percept annot
                current.DelAnnot(TPercept);
                // Remove from BB
                bb.RemoveFromEntry(current);
            }

            public void Reset()
            {
                i.Reset();
            }
        }

        private class IEnumeratorEnumerator : IEnumerator<Literal>
        {
            public IEnumerator<Dictionary<PredicateIndicator, BelEntry>> ins;
            public HashSet<Literal> percepts;
            public int size;
            IEnumerator<BelEntry> ibe = null;
            IEnumerator<Literal> il = null;
            IEnumerator<Literal> ilr = null;
            Literal l = null;
            public IEnumeratorEnumerator(IEnumerator<Dictionary<PredicateIndicator, BelEntry>> ins, HashSet<Literal> percepts, int size)
            {
                this.ins = ins;
                this.percepts = percepts;
                this.size = size;
                ibe = ins.Current.Values.GetEnumerator();
                il = null;
                ilr = null;
                l = null;
                GoNext();
            }
            

            public Literal Current => il.Current;

            object IEnumerator.Current => il.Current;

            //static IEnumeratorEnumerator(){ GoNext(); }

            public bool HasNext() => il != null && il.MoveNext();

            private void GoNext()
            {
                while (il == null || !il.MoveNext())
                {
                    if (ibe.MoveNext()) il = ibe.Current.list.GetEnumerator();
                    else if (ins.MoveNext()) ibe = ins.Current.Values.GetEnumerator();
                    else return;
                }
            }

            public Literal Next()
            {
                l = il.Current;
                ilr = il;
                GoNext();
                return l;
            }

            public void Remove()
            {
                ilr.Dispose();
                if (l.HasAnnot(TPercept)) percepts.Remove(l);
                size--;
            }

            public bool MoveNext()
            {
                return il.MoveNext();
            }

            public void Reset()
            {
                il.Reset();
            }

            public void Dispose()
            {
                il.Dispose();
            }
        }
    }
}
