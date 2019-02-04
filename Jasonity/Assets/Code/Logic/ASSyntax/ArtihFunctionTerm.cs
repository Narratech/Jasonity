using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.NoValueException;
import jason.asSemantics.Agent;
import jason.asSemantics.ArithFunction;
import jason.asSemantics.Unifier;

import java.util.Iterator;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

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
    public class ArtihFunctionTerm: Structure, NumberTerm
    {
    }
}
