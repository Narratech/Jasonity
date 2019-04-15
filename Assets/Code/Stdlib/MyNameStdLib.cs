using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.my_name</code></b>.
  <p>Description: gets the agent's unique identification in the
  multi-agent system. This identification is given by the runtime
  infrastructure of the system (centralised, saci, jade, ...).
  <p>Parameter:<ul>
  <li>+/- name (atom or variable): if this is a variable, unifies the agent
  name and the variable; if it is an atom, succeeds if the atom is equal to
  the agent's name.<br/>
  </ul>
  <p>Example:<ul>
  <li> <code>.my_name(N)</code>: unifies <code>N</code> with the
  agent's name.</li>
  <li> <code>.my_name(bob)</code>: true if the agent's name is \"bob\".</li>
  </ul>
 */

namespace Assets.Code.Stdlib
{
    public class MyNameStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new MyNameStdLib();
            return singleton;
        }

        override public int GetMinArgs()
        {
            return 1;
        }
        override public int GetMaxArgs()
        {
            return 1;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return un.Unifies(args[0], new Atom(ts.GetUserAgArch().getAgName()));
        }
    }
}
