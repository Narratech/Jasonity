using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    public class Rule : Term
    {
        private bool agentBelievesThisIsTrue;
        private Dictionary<Literal, string> conditions;
        private Literal belief;

        //Constructor for beliefs and objectives rules 
        public Rule(Dictionary<Literal, string> terms, bool believes)
        {
            this.agentBelievesThisIsTrue = believes;
            this.belief = terms.First().Key;
            terms.Remove(belief);
            this.conditions = terms;
        }

        public override bool IsRule()
        {
            return true;
        }

        public Literal Belief { get => this.belief; }

        public Dictionary<Literal, string> Conditions { get => this.conditions; }
    }
}
