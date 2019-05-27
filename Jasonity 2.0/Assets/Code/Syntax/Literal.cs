using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Literal : Term
    {
        private bool agentBelievesThisIsTrue;
        private string functor;
        
        public Literal(string functor, bool believes)
        {
            this.agentBelievesThisIsTrue = believes;
            this.functor = functor;
        }

        public bool AgentBelievesThisIsTrue { get => this.agentBelievesThisIsTrue; }

        public string Functor { get => this.functor; }

        public override bool IsLiteral()
        {
            return true;
        }
    }
}
