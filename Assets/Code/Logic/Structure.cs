using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Structure : Literal
    {
        List<Term> parameters;

        public Structure(string functor, bool b, List<Term> args) : base(functor, b)
        {
            parameters = new List<Term>();
            foreach (Term term in args)
                parameters.Add(term);
        }
    }
}
