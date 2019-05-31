using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Trigger
    {
        private string trigger;
        private readonly char @operator; //+ or -
        private List<string> parameters;


        public Trigger(char @operator, string trigger, string[] parameters)
        {
            this.@operator = @operator;
            this.trigger = trigger;
            this.parameters = parameters.ToList();
        }

        public char Operator { get => @operator; }

        public string GetTrigger { get => trigger; }
    }
}
