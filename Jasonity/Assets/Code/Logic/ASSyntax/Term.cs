using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Unifier;
import jason.util.ToDOM;

import java.io.Serializable;
import java.util.Map;

/**
 * Common interface for all kind of terms
 *
 * @opt nodefillcolor lightgoldenrodyellow
 */
namespace Jason.Logic.AsSyntax
{
    [Serializable]
    interface Term: ICloneable, IComparable<Term>, ToDOM
    {
    }
}
