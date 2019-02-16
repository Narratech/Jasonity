// Implements a default Belief Base
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

        public List<Literal> percepts = new List<Literal>();
        private object TPercept;

        public DefaultBeliefBase()
        {
            // ??? nameSpaces = (Literal.DefaultNS, belsMapDefaultNS);
        }

        public bool Add(Literal l)
        {
            return Add(l);
        }

        public void Clear()
        {
            size = 0;
            percepts.Clear();
            belsMapDefaultNS.Clear();
            nameSpaces.Clear();
            // ??? nameSpaces = (Literal.DefaultNS, belsMapDefaultNS);
        }

        public Literal Contains(Literal l)
        {
            throw new System.NotImplementedException();
        }

        public void Init(Agent ag, string[] args)
        {
            throw new System.NotImplementedException();
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
                /* ???????
                Dictionary<PredicateIndicator, BelEntry> belsMap = l.getNS() == Literal.DefaultNS ? belsMapDefaultNS : nameSpaces.???;
                PredicateIndicator key = l.GetPredicateIndicator();
                BelEntry entry = belsMap.???
                entry.Remove(l);
                if (entry.IsEmpty())
                {
                    belsMap.Remove(key);
                }
                size--; */
                return true;
            }
        }

        public int Size()
        {
            return size;
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        class BelEntry
        {
            private List<Literal> list = new List<Literal>();
            private Dictionary<StructureWrapperForLiteral, Literal> map = new Dictionary<StructureWrapperForLiteral, Literal>();

            public void Add(Literal l)
            {
                map.Add(new StructureWrapperForLiteral(l), l);
                list.Add(l);
            }

            public void Remove(Literal l)
            {
                Literal linmap = map.Remove(new StructureWrapperForLiteral(l));
                if (linmap != null)
                {
                    list.Remove(linmap);
                }
            }

            public int Size()
            {
                return map.Count; // ???
            }

            public bool IsEmpty()
            {
                return list.Count == 0;
            }

            public Literal Contains(Literal l)
            {
                return map.GetObjectData(new StructureWrapperForLiteral(l));
            }
        }
    }
}
