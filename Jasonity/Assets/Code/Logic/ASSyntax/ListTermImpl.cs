using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Agent;
import jason.asSemantics.Unifier;
import jason.asSyntax.parser.as2j;

import java.io.StringReader;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.ListIterator;
import java.util.Set;
import java.util.TreeSet;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;


/**
 * Represents a list node as in prolog .(t1,.(t2,.(t3,.))).
 *
 * Each nth-ListTerm has both a term and the next ListTerm.
 * The last ListTem is an empty ListTerm (term==null).
 * In lists terms with a tail ([a|X]), next is the Tail (next==X, term==a).
 *
 * @navassoc - element - Term
 * @navassoc - next - ListTerm
 *
 */
namespace Jason.Logic.AsSyntax
{
    public class ListTermImpl: Structure, ListTerm
    {
    }
}
