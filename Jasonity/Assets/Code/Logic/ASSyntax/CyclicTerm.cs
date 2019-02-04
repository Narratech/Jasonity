using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import org.w3c.dom.Document;
import org.w3c.dom.Element;

import jason.asSemantics.Unifier;



/**
 * A term with recursion (cyclic), created by code like X = f(X).
 */

namespace Jason.Logic.AsSyntax
{
    public class CyclicTerm: LiteralImpl
    {
    }
}
