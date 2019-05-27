using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Trigger:Term
    {
        private Term trigger;
        private readonly char @operator;

        public Trigger(char @operator, bool believes, Term trigger)
        {
            this.@operator = @operator;
            this.trigger = trigger;
        }

        public override bool IsTrigger()
        {
            return true;
        }

        public char Operator { get => @operator; }

        public Term Triggerr { get => trigger; }
    }
}
