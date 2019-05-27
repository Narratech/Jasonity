using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Predicate : Literal
    {
        private List<Term> arguments;
        private List<Term> annotations;

        public Predicate(string functor, bool believes, List<Term> args, List<Term> annots) : 
            base(functor, believes)
        {
            this.arguments = new List<Term>();
            foreach (Term term in args)
                this.arguments.Add(term);

            this.annotations = new List<Term>();
            foreach (Term term in annots)
                this.annotations.Add(term);
        }

        public List<Term> Arguments { get => arguments; }

        public List<Term> Annotations { get => annotations; }
    }
}
