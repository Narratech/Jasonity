using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Unifier;

import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

/**
     A rule is a Literal (head) with a body, as in "a :- b &amp; c".

     @navassoc - body - LogicalFormula
 */
namespace Jason.Logic.AsSyntax
{
    public class Rule: LiteralImpl
    {
    }
}
