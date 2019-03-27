using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public enum TEType { belief, achieve, test };

    class Trigger
    {
        private object add;
        private TEType belief;
        private Literal lAdd;

        public Trigger(object add, TEType belief, Literal lAdd)
        {
            this.add = add;
            this.belief = belief;
            this.lAdd = lAdd;
        }

        public Literal GetLiteral()
        {
            throw new NotImplementedException();
        }

        public TEType GetType()
        {
            throw new NotImplementedException();
        }

        public bool IsAddition()
        {
            throw new NotImplementedException();
        }

        public bool IsGoal()
        {
            throw new NotImplementedException();
        }

        internal object GetNS()
        {
            throw new NotImplementedException();
        }

        public bool IsMetaEvent()
        {
            throw new NotImplementedException();
        }

        internal PredicateIndicator GetPredicateIndicator()
        {
            throw new NotImplementedException();
        }

        internal Trigger Clone()
        {
            throw new NotImplementedException();
        }
    }
}
