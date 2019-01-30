using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jason.AsSemantics.Entities
{
    [Serializable]
    public abstract class DefaultArithFunction : ArithFunction
    {
        public bool allowUngroundTerms()
        {
            return false;
        }

        public bool checkArity(int a)
        {
            return true;
        }

        public double evaluate(TransitionSystem ts, Term[] args)
        {
            return 0;
        }

        public string getName()
        {
            return getClass().getName();
        }

        public override string ToString()
        {
            return "function " + getName();
        }
    }
}
