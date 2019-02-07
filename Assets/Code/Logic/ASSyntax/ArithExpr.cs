using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import java.io.StringReader;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

import jason.NoValueException;
import jason.asSemantics.Agent;
import jason.asSemantics.Unifier;
import jason.asSyntax.parser.as2j;

/**
  Represents and solve arithmetic expressions like "10 + 30".
 */
namespace Jason.Logic
{
    public class ArithExpr: ArithFunctionTerm, NumberTerm
    {

    }
}
