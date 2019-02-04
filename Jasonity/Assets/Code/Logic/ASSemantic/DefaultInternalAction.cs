using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.JasonException;
import jason.asSyntax.Literal;
import jason.asSyntax.Term;

import java.io.Serializable;

/**
 * Default implementation of the internal action interface (it simply returns false
 * for the interface methods).
 *
 */
namespace Logica.ASSemantic
{
    [Serializable]
    class DefaultInternalAction: InternalAction
    {
    }
}
