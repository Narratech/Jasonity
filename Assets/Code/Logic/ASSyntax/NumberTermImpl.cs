using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/** Immutable class that implements a term that represents a number */

namespace Jason.Logic.AsSyntax
{
    public sealed class NumberTermImpl : DefaultTerm, NumberTerm
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
