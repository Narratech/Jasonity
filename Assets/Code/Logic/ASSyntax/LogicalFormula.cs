using Logica.ASSemantic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Represents a logical formula (p, p & q, not p, 3 > X, ...) which can be
 * evaluated into a truth value.
 *
 * @opt nodefillcolor lightgoldenrodyellow
 *
 */
namespace Jason.Logic.AsSyntax
{
    public interface LogicalFormula: Term, ICloneable
    {
        /**
     * Checks whether the formula is a
     * logical consequence of the belief base.
     *
     * Returns an iterator for all unifiers that are consequence.
     */
        IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un);
    }
}
