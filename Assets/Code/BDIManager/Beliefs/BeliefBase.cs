// Implements a default Belief Base
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.Logic;
using Assets.Code.Logic.AsSyntax;
using Assets.Code.ReasoningCycle;

namespace BDIManager.Beliefs
{
    public class BeliefBase
    {
        private Dictionary<Atom, Dictionary<PredicateIndicator, BelEntry>> nameSpaces = new Dictionary<Atom, Dictionary<PredicateIndicator, BelEntry>>();
        private Dictionary<PredicateIndicator, BelEntry> belsMapDefaultNS = new Dictionary<PredicateIndicator, BelEntry>();

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

        // Returns the namespaces
        public HashSet<Atom> GetNameSpaces()
        {
            return new HashSet<Atom>(nameSpaces.Keys);
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
            return new IEnumeratorGetPercepts(i);
        } 

        // Returns percepts
        public HashSet<Literal> GetPerceptsSet()
        {
            return percepts;
        }

        // Adds a new literal
        public bool Add(Literal l)
        {
            return Add(l, false);
        }

        // Adds a new literal in a specific index
        public bool Add(int index, Literal l)
        {
            return Add(l, index != 0);
        }

        internal void Init(Agent ag, object p)
        {
            throw new NotImplementedException();
        }

        // Adds a new literal at the end of the belief base
        protected bool Add(Literal l, bool addInEnd)
        {
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
                    bool result = bl.DelAnnots(l.GetAnnots());
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

        // Each predicate indicator has one BelEntry assigned
        class BelEntry
        {
            private List<Literal> list = new List<Literal>(); // Keeps the order of the beliefs
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
        }

        internal IEnumerator<Literal> GetCandidateBeliefs(Literal literal, Unifier un)
        {
            throw new NotImplementedException();
        }

        private class IEnumeratorGetPercepts : IEnumerator<Literal>
        {
            private IEnumerator<Literal> i;
            public IEnumeratorGetPercepts(IEnumerator<Literal> i)
            {
                this.i = i;
            }

            public Literal Current => null;

            object IEnumerator.Current => i.Current;

            public void Dispose()
            {
                if (Current == null)
                {

                }
                i.Dispose();
                Current.DelAnnot(TPercept);
                RemoveFromEntry(Current);
            }

            public bool MoveNext()
            {
                return i.MoveNext();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
