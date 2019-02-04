using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Agent;
import jason.asSemantics.Unifier;

import java.util.Iterator;

/**
 * Represents a logical formula (p, p & q, not p, 3 > X, ...) which can be
 * evaluated into a truth value.
 *
 * @opt nodefillcolor lightgoldenrodyellow
 *
 */
namespace Jason.Logic.AsSyntax
{
    interface LogicalFormula: Term, ICloneable
    {
    }
}
