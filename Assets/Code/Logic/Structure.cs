using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    public class Structure : Literal
    {
        List<Term> arguments;

        public Structure(string functor, bool b, List<Term> args) : base(functor, b)
        {
            arguments = new List<Term>();
            foreach (Term term in args)
                arguments.Add(term);
        }
    }
}
