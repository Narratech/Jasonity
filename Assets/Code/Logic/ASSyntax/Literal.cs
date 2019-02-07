using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.JasonException;
import jason.architecture.AgArch;
import jason.asSemantics.Agent;
import jason.asSemantics.Unifier;
import jason.asSyntax.parser.as2j;

import java.io.StringReader;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 This class represents an abstract literal (an Atom, Structure, Predicate, etc), it is mainly
 the interface of a literal.

 To create a new Literal, one of the following concrete classes may be used:
 <ul>
 <li> Atom -- the most simple literal, is composed by only a functor (no term, no annots);
 <li> Structure -- has functor and terms;
 <li> Pred -- has functor, terms, and annotations;
 <li> LiteralImpl -- Pred + negation.
 </ul>
 The latter class supports all the operations of
 the Literal interface.

 <p>There are useful static methods in class {@link ASSyntax} to create Literals.

 @navassoc - type - PredicateIndicator
 @opt nodefillcolor lightgoldenrodyellow

 @see ASSyntax
 @see Atom
 @see Structure
 @see Pred
 @see LiteralImpl

 */
namespace Jason.Logic.AsSyntax
{
    public abstract class Literal: DefaultTerm, LogicalFormula
    {
    }
}
