using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Immutable class for string terms.
 *
 */
namespace Jason.Logic.AsSyntax
{
    public sealed class StringTermImpl : DefaultTerm, StringTerm
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
