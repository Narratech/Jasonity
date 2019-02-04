using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Agent;
import jason.asSemantics.Unifier;

import java.util.Iterator;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

/**
 *  Represents a plan body item (achieve, test, action, ...) and its successors.
 *
 *  A plan body like <code>a1; ?t; !g</code> is represented by the following structure
 *  <code>(a1, (?t, (!g)))</code>.
 *
 *
 *  @navassoc - next - PlanBody
 *  @navassoc - type - PlanBody.BodyType
 *
 *  @author Jomi
 */
namespace Jason.Logic.AsSyntax
{
    public class PlanBodyImpl: Structure, PlanBody //Iterable<PlanBody>
    {
    }
}
