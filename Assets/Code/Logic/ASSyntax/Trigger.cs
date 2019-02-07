using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Unifier;
import jason.asSyntax.PlanBody.BodyType;
import jason.asSyntax.parser.ParseException;
import jason.asSyntax.parser.as2j;
import jason.bb.BeliefBase;

import java.io.StringReader;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

/**
  Represents an AgentSpeak trigger (like +!g, +p, ...).

  It is composed by:
     an operator (+ or -);
     a type (<empty>, !, or ?);
     a literal

  (it extends structure to be used as a term)

  @opt attributes
  @navassoc - literal  - Literal
  @navassoc - operator - TEOperator
  @navassoc - type     - TEType
*/
namespace Jason.Logic.AsSyntax
{
    public class Trigger: Structure, ICloneable
    {
    }
}
