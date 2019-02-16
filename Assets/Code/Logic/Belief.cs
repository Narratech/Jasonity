using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    /*Class Belief*/
    public class Belief : Term
    {
        /*This is the subject who has this belief*/
        private Subject subject;

        /*Constructor
            name: string
            subject: string
        */
        public Belief(string name, string subject) : base(name)
        {
            this.subject = new Subject(subject);
        }

        public override bool IsBelief()
        {
            return true;
        }

        /*Get for the subject*/
        public Subject Subject { get => this.subject; }
    }
}
