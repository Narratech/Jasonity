using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Variable : DefaultTerm
    {
        private string var;

        public Variable(string name)
        {
            this.var = name;
        }

        public string Var { get => this.var; set => this.var = value; }
    }
}
