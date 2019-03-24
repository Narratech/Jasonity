using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Rule : DefaultTerm
    {
        Dictionary<Literal, string> condition;
        Literal belief;

        //Constructor for beliefs and objectives rules 
        public Rule(Dictionary<Literal, string> terms, bool b)
        {
            this.belief = terms.First().Key;
            terms.Remove(belief);
            this.condition = terms;
        }

        public override bool IsRule()
        {
            return true;
        }

        public Literal Belief { get => this.belief; }

        public Dictionary<Literal, string> Condition { get => this.condition; }
    }
}