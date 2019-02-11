using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Represents an arithmetic function, like math.max(arg1,arg2) -- a functor (math.max) and two arguments.
 * A Structure is thus used to store the data.
 *
 * @composed - "arguments (from Structure.terms)" 0..* Term
 *
 *
 */

namespace Jason.Logic.AsSyntax
{
    public class ArithFunctionTerm : Structure, NumberTerm
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Term other)
        {
            throw new NotImplementedException();
        }
    }
}
