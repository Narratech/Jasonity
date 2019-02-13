using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Belief : Term
    {
        private Subject subject;

        public Belief(string name, string subject) : base(name)
        {
            this.subject = new Subject(subject);
        }

        public override bool IsBelief()
        {
            return true;
        }

        public Subject Subject { get => this.subject; }
    }
}
