// Implements a default Belief Base
using System;
using System.Collections.Generic;
using Assets.Code.Agent;
using Assets.Code.Logic;

namespace BDIManager.Beliefs
{
    class DefaultBeliefBase : BeliefBase
    {
        private Dictionary<Atom, Dictionary<PredicateIndicator, BelEntry>> nameSpaces = new Dictionary<Atom, Dictionary<PredicateIndicator, BelEntry>>();
        private Dictionary<PredicateIndicator, BelEntry> belsMapDefaultNS = new Dictionary<PredicateIndicator, BelEntry>();

        private int size = 0;

        public HashSet<Literal> percepts = new HashSet<Literal>();
        private object TPercept;

        public DefaultBeliefBase()
        {
            nameSpaces.Add(Literal.DefaultNS, belsMapDefaultNS);    // Literal.DefaultNS must be Atom
        }

        public HashSet<Atom> GetNameSpaces()
        {
            return new HashSet<Atom>(nameSpaces.Keys);
        }

        public int Size()
        {
            return size;
        }

        public void Clear()
        {
            size = 0;
            percepts.Clear();
            belsMapDefaultNS.Clear();
            nameSpaces.Clear();
            nameSpaces.Add(Literal.DefaultNS, belsMapDefaultNS);    // Literal.DefaultNS must be Atom
        }

        public IEnumerable<Literal> GetPercepts()
        {
            throw new NotImplementedException();
        } 

        public HashSet<Literal> GetPerceptsSet()
        {
            return percepts;
        }

        public bool Add(Literal l)
        {
            return Add(l, false);
        }

        public bool Add(int index, Literal l)
        {
            return Add(l, index != 0);
        }

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
                belsMap = nameSpaces[l.GetNS()]; // ???
                if (belsMap == null)
                {
                    belsMap = new Dictionary<PredicateIndicator, BelEntry>();
                    nameSpaces.Add(l.GetNS(), belsMap);
                }
            }
            BelEntry entry = belsMap.GetObjectData(l.GetPredicateIndicator());
            if (entry == null)
            {
                entry = new BelEntry();
                belsMap.Add(l.GetPredicateIndicator(), entry);
            }
            return entry;
        }

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

        public Literal Contains(Literal l)
        {
            Dictionary<PredicateIndicator, BelEntry> belsMap = l.GetNS() == Literal.DefaultNS ? belsMapDefaultNS : nameSpaces[l.GetNS()]);
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

        class BelEntry
        {
            private List<Literal> list = new List<Literal>();
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
                    list.Insert(list.Count, l); // ???
                }
            }

            public void Remove(Literal l)
            {
                Literal linmap = map.Remove(new StructureWrapperForLiteral(l)); // ???
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
    }
}
