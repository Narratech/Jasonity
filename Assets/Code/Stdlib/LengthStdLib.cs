using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.length</code></b>.
  <p>Description: gets the length of strings or lists.
  <p>Parameters:<ul>
  <li>+ argument (string or list): the term whose length is to be determined.<br/>
  <li>+/- length (number).
  </ul>
  <p>Examples:<ul>
  <li> <code>.length("abc",X)</code>: <code>X</code> unifies with 3.
  <li> <code>.length([a,b],X)</code>: <code>X</code> unifies with 2.
  <li> <code>.length("a",2)</code>: false.
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class LengthStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
                singleton = new LengthStdLib();
            return singleton;
        }

        override public int GetMinArgs()
        {
            return 2;
        }

        override public int GetMaxArgs()
        {
            return 2;
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ITerm l1 = args[0];
            ITerm l2 = args[1];

            INumberTerm size = null;
            if (l1.IsList())
            {
                IListTerm lt = (IListTerm)l1;
                size = new NumberTermImpl(lt.Size());
            }
            else if (l1.IsString())
            {
                IStringTerm st = (IStringTerm)l1;
                size = new NumberTermImpl(st.GetString().Length);
            }
            if (size != null)
            {
                return un.Unifies(l2, size);
            }
            return false;
        }
    }
}
