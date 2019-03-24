using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Structure : Literal
    {
        List<DefaultTerm> parameters;

        public Structure(string functor, bool b, List<DefaultTerm> args) : base(functor, b)
        {
            parameters = new List<DefaultTerm>();
            foreach (DefaultTerm term in args)
                parameters.Add(term);
        }
    }
}
