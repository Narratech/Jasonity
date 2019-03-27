using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.ReasoningCycle;

namespace Assets.Code.Logic
{
    public abstract class Term
    {
        //private string @class;
        private string name;

        public Term(string name)
        {
            //this.@class = @class;
            this.name = name;
        }

        public virtual bool IsBelief()
        {
            return false;
        }

        public virtual bool IsSubject()
        {
            return false;
        }

        public virtual bool IsObjective()
        {
            return false;
        }

        public virtual bool IsObjectOfTheObjective()
        {
            return false;
        }

        public virtual bool IsPlan()
        {
            return false;
        }

        public string Name { get => this.name; }

        internal bool IsPlanBody()
        {
            throw new NotImplementedException();
        }

        internal Term GetLiteral()
        {
            throw new NotImplementedException();
        }

        internal bool IsCyclicTerm()
        {
            throw new NotImplementedException();
        }

        internal VarTerm GetCyclicVar()
        {
            throw new NotImplementedException();
        }

        internal bool IsVar()
        {
            throw new NotImplementedException();
        }

        internal bool HasAnnot()
        {
            throw new NotImplementedException();
        }

        internal bool IsPred()
        {
            throw new NotImplementedException();
        }

        internal Pred Clone()
        {
            throw new NotImplementedException();
        }

        internal bool IsArithExpr()
        {
            throw new NotImplementedException();
        }

        internal Term Capply(Unifier unifier)
        {
            throw new NotImplementedException();
        }
    }
}
