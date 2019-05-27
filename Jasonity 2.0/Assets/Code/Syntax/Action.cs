using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    public class Action
    {
        private string action;
        private string[] arguments;
        private char symbol;

        public Action(string a, string[] args, char symbol)
        {
            this.action = a;
            this.arguments = args;
            this.symbol = symbol;
        }

        public string Actiom { get => action; }
        public string[] Arguments { get => arguments; }
        public char Symbol { get => symbol; }
    }
}
