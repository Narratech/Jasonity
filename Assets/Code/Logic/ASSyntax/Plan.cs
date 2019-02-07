using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Unifier;
import jason.asSyntax.parser.as2j;

import java.io.Serializable;
import java.io.StringReader;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

/** Represents an AgentSpack plan
    (it extends structure to be used as a term)

 @navassoc - label - Pred
 @navassoc - event - Trigger
 @navassoc - context - LogicalFormula
 @navassoc - body - PlanBody
 @navassoc - source - SourceInfo

 */
namespace Jason.Logic.AsSyntax
{
    [Serializable]
    public class Plan: Structure, ICloneable
    {
    }
}
