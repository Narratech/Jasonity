using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.NoValueException;
import jason.asSemantics.Agent;
import jason.asSemantics.Unifier;
import jason.asSyntax.parser.as2j;

import java.io.StringReader;
import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.ListIterator;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

/**
 * Represents a variable Term: like X (starts with upper case). It may have a
 * value, after {@link VarTerm}.apply.
 *
 * An object of this class can be used in place of a
 * Literal, Number, List, String, .... It behaves like a
 * Literal, Number, .... just in case its value is a Literal,
 * Number, ...
 *
 */
namespace Jason.Logic.AsSyntax
{
    public class VarTerm: LiteralImpl, NumberTerm, ListTerm
    {
    }
}
