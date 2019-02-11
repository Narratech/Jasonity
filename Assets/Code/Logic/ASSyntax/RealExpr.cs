using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
 * Represents a relational expression like 10 > 20.
 *
 * When the operator is <b>=..</b>, the first argument is a literal and the
 * second as list, e.g.:
 * <code>
 * Literal =.. [functor, list of terms, list of annots]
 * </code>
 * Examples:
 * <ul>
 * <li> X =.. [~p, [t1, t2], [a1,a2]]<br>
 *      X is ~p(t1,t2)[a1,a2]
 * <li> ~p(t1,t2)[a1,a2] =.. X<br>
 *      X is [~p, [t1, t2], [a1,a2]]
 * </ul>
 *
 * @navassoc - op - RelationalOp
 *
 */
namespace Jason.Logic.AsSyntax
{
    public class RealExpr : BinaryStructure, LogicalFormula
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
