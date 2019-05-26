using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    public class Trigger:Literal
    {
        private readonly char @operator;

        public Trigger(char @operator, bool believes, Literal literal):
            base(literal.Functor, believes)
        {
            this.@operator = @operator;
        }

        public override bool IsTrigger()
        {
            return true;
        }

        public char Operator { get => @operator; }
    }
}
