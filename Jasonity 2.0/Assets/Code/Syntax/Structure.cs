using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Structure : Literal
    {
        private List<Term> parameters;

        public Structure(string functor, bool believes, List<Term> args) :
            base(functor, believes)
        {
            parameters = new List<Term>();
            foreach (Term term in args)
                parameters.Add(term);
        }

        public List<Term> Parameters { get => parameters; }
    }
}
