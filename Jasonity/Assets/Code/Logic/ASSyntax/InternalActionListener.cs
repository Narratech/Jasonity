using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import java.util.ConcurrentModificationException;
import java.util.Iterator;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

import jason.asSemantics.Agent;
import jason.asSemantics.InternalAction;
import jason.asSemantics.Unifier;
import jason.stdlib.puts;


/**
 A particular type of literal used to represent internal actions (which has a "." in the functor).

 @navassoc - ia - InternalAction

 */
namespace Jason.Logic.AsSyntax
{
    public class InternalActionListener: Structure, LogicalFormula 
    {
    }
}
