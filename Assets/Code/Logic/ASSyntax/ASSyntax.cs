using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import jason.NoValueException;
import jason.asSyntax.parser.ParseException;
import jason.asSyntax.parser.as2j;
import jason.asSyntax.parser.as2jConstants;

/**
  Factory for objects used in Jason AgentSpeak syntax.

  <p><b>Examples of Term</b>:
  <pre>
  import static jason.asSyntax.ASSyntax.*;

  ...
  Atom       a = createAtom("a");
  NumberTerm n = createNumber(3);
  StringTerm s = createString("s");
  Structure  t = createStructure("p", createAtom("a")); // t = p(a)
  ListTerm   l = createList(); // empty list
  ListTerm   f = createList(createAtom("a"), createStructure("b", createNumber(5))); // f = [a,b(5)]

  // or use a parsing (easier but slower)
  Term n = parseTerm("5");
  Term t = parseTerm("p(a)");
  Term l = parseTerm("[a,b(5)]");
  </pre>

  <p><b>Examples of Literal</b>:
  <pre>
  import static jason.asSyntax.ASSyntax.*;

  ...
  // create the literal 'p'
  Literal l1 = createLiteral("p");

  // create the literal 'p(a,3)'
  Literal l2 = createLiteral("p", createAtom("a"), createNumber(3));

  // create the literal 'p(a,3)[s,"s"]'
  Literal l3 = createLiteral("p", createAtom("a"), createNumber(3))
                            .addAnnots(createAtom("s"), createString("s"));

  // create the literal '~p(a,3)[s,"s"]'
  Literal l4 = createLiteral(Literal.LNeg, "p", createAtom("a"), createNumber(3))
                            .addAnnots(createAtom("s"), createString("s"));

  // or use the parser (easier but slower)
  Literal l4 = parseLiteral("~p(a,3)[s]");
  </pre>

 */
namespace Jason.Logic.AsSyntax
{
    public class ASSyntax
    {
    }
}
