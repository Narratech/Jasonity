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
<p>Internal action: <b><code>.min</code></b>.
    <p>Description: gets the minimum value of a list of terms, using
    the "natural" order of terms. Between
different types of terms, the following order is
used:<br>
numbers &lt; atoms &lt; structures &lt; lists
<p>Parameters:<ul>
<li>+   list (list): the list where to find the minimal term.<br/>
<li>+/- minimal (term).
</ul>
<p>Examples:<ul>
<li> <code>.min([c,a,b],X)</code>: <code>X</code> unifies with
<code>a</code>.
<li>
<code>.min([b,c,10,g,f(10),[3,4],5,[3,10],f(4)],X)</code>:
<code>X</code> unifies with <code>5</code>.
<li>
<code>.min([3,2,5],2)</code>: true.
<li>
<code>.min([3,2,5],5)</code>: false.
<li>
<code>.min([],X)</code>: false.
</ul>
*/

namespace Assets.Code.Stdlib
{
    public class MinStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new MinStdLib();
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

            IListTerm list = (IListTerm)args[0];
            if (list.Count == 0) {
                return false;
            }

            IEnumerator<ITerm> i = list.GetEnumerator();
            ITerm min = i.Current;
            while (i.MoveNext()) {
                ITerm t = i.Current;
                if (Compare(min, t)) {
                    min = t;
                }
            }
            return un.Unifies(args[1], (ITerm)min.Clone()); // Como uso el Clone de C# lo que clono son object que luego hay que castear...
        }

        protected virtual bool Compare(ITerm a, ITerm t)
        {
            return a.CompareTo(t) > 0;
        }
    }
}
