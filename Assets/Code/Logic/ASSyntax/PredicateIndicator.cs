using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import java.io.Serializable;

/**
 * Represents the "type" of a predicate based on the functor and the arity, e.g.: ask/4
 *
 */
namespace Jason.Logic.AsSyntax
{
    [Serializable]
    public sealed class PredicateIndicator: IComparable<PredicateIndicator>
    {
    }
}
