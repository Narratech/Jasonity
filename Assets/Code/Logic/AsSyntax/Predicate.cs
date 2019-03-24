using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Predicate : Literal
    {
        List<DefaultTerm> arguments;
        List<DefaultTerm> annotations;

        public Predicate(string functor, bool b, List<DefaultTerm> args, List<DefaultTerm> annots) : base(functor, b)
        {
            this.arguments = new List<DefaultTerm>();
            foreach (DefaultTerm term in args)
                this.arguments.Add(term);

            this.annotations = new List<DefaultTerm>();
            foreach (DefaultTerm term in annots)
                this.annotations.Add(term);
        }
    }
}
