using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
