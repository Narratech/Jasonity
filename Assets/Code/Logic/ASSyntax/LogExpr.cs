using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Agent;
import jason.asSemantics.Unifier;
import jason.asSyntax.parser.as2j;

import java.io.StringReader;
import java.util.Collections;
import java.util.Iterator;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;


/**
   Represents a logical formula with some logical operator ("&amp;",  "|", "not").

   @navassoc - op - LogicalOp

 */
namespace Jason.Logic.AsSyntax
{
    public class LogExpr: BinaryStructure, LogicalFormula
    {
    }
}
