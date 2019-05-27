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
        private Literal objective;

        //Constructor for beliefs and objectives rules 
        public Rule(Dictionary<Literal, string> terms, bool believes)
        {
            this.agentBelievesThisIsTrue = believes;
            this.objective = terms.First().Key;
            terms.Remove(objective);
            this.conditions = terms;
        }

        public override bool IsRule()
        {
            return true;
        }

        public Literal Objective { get => this.objective; }

        public Dictionary<Literal, string> Conditions { get => this.conditions; }
    }
}
