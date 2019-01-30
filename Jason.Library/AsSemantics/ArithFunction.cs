using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jason.AsSemantics.Entities
{
    public interface ArithFunction
    {
        /** returns the name of the function */
        string getName();

        /** evaluates/computes the function based on the args */
        double evaluate(TransitionSystem ts, Term[] args) throws Exception;

        /** returns true if a is a good number of arguments for the function */
        bool checkArity(int a);

        /** returns true if the arguments of the function can be unground (as in .count) */
        bool allowUngroundTerms();
    }
}
