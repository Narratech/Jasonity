using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

/** Immutable class that implements a term that represents a number */

namespace Jason.Logic.AsSyntax
{
    public sealed class NumberTermImpl: DefaultTerm, NumberTerm
    {
    }
}
