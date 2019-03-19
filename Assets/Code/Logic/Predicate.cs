using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Predicate : Literal
    {
        List<Term> arguments;
        List<Term> annotations;

        public Predicate(string functor, bool b, List<Term> args, List<Term> annots) : base(functor, b)
        {
            this.arguments = new List<Term>();
            foreach (Term term in args)
                this.arguments.Add(term);

            this.annotations = new List<Term>();
            foreach (Term term in annots)
                this.annotations.Add(term);
        }
    }
}
