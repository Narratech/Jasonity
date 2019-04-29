using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.sort</code></b>.
  <p>Description: sorts a list of terms. The "natural" order for each type of
  terms is used. Between different types of terms, the following order is
  used:<br>
  numbers &lt; strings &lt; lists &lt; literals (by negation, arity, functor, terms, annotations) &lt; variables
  <p>Parameters:<ul>
  <li>+   unordered list (list): the list the be sorted.<br/>
  <li>+/- ordered list (list): the sorted list.
  </ul>
  <p>Examples:<ul>
  <li> <code>.sort([c,a,b],X)</code>: <code>X</code> unifies with
  <code>[a,b,c]</code>.
  <li>
  <code>.sort([C,b(4),A,4,b(1,1),"x",[],[c],[a],[b,c],[a,b],~a(3),a(e,f),b,a(3),b(3),a(10)[30],a(10)[5],a,a(d,e)],X)</code>:
  <code>X</code> unifies with
  <code>[4,"x",[],[a],[c],[a,b],[b,c],a,b,a(3),a(10)[5],a(10)[30],b(3),b(4),a(d,e),a(e,f),b(1,1),~a(3),A,C]</code>.
  <li>
  <code>.sort([3,2,5],[2,3,5])</code>: true.
  <li>
  <code>.sort([3,2,5],[a,b,c])</code>: false.
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class SortStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new SortStdLib();
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

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[0].IsList())
                throw JasonityException.CreateWrongArgument(this,"first argument must be a list");
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            List<ITerm> l = ((IListTerm)args[0]).GetAsList();
            l.Sort();
            return un.Unifies(AsSyntax.AsSyntax.CreateList(l), args[1]);
        }
    }
}
