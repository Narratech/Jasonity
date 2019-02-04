using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

import jason.asSemantics.Unifier;
import jason.asSyntax.parser.as2j;
import jason.util.Config;

/**
 * Represents a structure: a functor with <i>n</i> arguments,
 * e.g.: val(10,x(3)).
 *
 * @composed - terms 0..* Term
 */
namespace Jason.Logic.AsSyntax
{
    public class Structure: Atom
    {

    }
}
