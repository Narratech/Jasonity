using Jason.Logic.AsSyntax.Directives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Agent;
import jason.asSyntax.ASSyntax;
import jason.asSyntax.Plan;
import jason.asSyntax.Pred;
import jason.asSyntax.SourceInfo;
import jason.asSyntax.Term;
import jason.asSyntax.directives.DefaultDirective;
import jason.asSyntax.directives.Directive;
import jason.asSyntax.directives.DirectiveProcessor;

import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Implementation of the  Open-Minded Commitment pattern (see DALT 2006 paper)
 *
 * @author jomi
 */

namespace Jason.Logic.AsSyntax.Patterns.Goal
{
    public class OMC: DefaultDirective, Directive
    {
    }
}
