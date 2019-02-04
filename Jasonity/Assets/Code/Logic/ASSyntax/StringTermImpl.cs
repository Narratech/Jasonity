using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSyntax.parser.as2j;

import java.io.StringReader;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

/**
 * Immutable class for string terms.
 *
 */
namespace Jason.Logic.AsSyntax
{
    public sealed class StringTermImpl: DefaultTerm, StringTerm
    {
    }
}
