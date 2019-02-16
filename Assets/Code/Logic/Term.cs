using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    /*Father class for all the terms of text file rules*/
    public abstract class Term
    {
        /*Name of the term*/
        private string name;

        public Term(string name)
        {
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
