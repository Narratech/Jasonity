using Jason.Logic.AsSyntax.Directives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Agent;
import jason.asSyntax.ASSyntax;
import jason.asSyntax.LogExpr;
import jason.asSyntax.LogicalFormula;
import jason.asSyntax.Pred;
import jason.asSyntax.StringTerm;
import jason.asSyntax.Term;
import jason.asSyntax.Trigger;
import jason.asSyntax.directives.DefaultDirective;
import jason.asSyntax.directives.Directive;

import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Implementation of the Sequenced Goal Adoption pattern (see DALT 2006 paper)
 *
 */

namespace Jason.Logic.AsSyntax.Patterns.Goal
{
    public class SGA: DefaultDirective, Directive
    {
    }
}
