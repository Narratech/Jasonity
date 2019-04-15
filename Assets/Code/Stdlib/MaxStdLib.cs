using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
<p>Internal action: <b><code>.max</code></b>.
<p>Description: gets the maximum value within a list of terms,
using the "natural" order for each type of term. Between
different types of terms, the following order is
used:<br>
numbers &lt; atoms &lt; structures &lt; lists
<p>Parameters:<ul>
<li>+   list (list): the list where to find the maximum term.<br/>
<li>+/- maximum (term).
</ul>
<p>Examples:<ul>
<li> <code>.max([c,a,b],X)</code>: <code>X</code> unifies with
<code>c</code>.
<li>
<code>.max([b,c,10,g,f(10),5,f(4)],X)</code>:
<code>X</code> unifies with <code>f(10)</code>.
<li>
<code>.max([3,2,5],2])</code>: false.
<li>
<code>.max([3,2,5],5)</code>: true.
<li>
<code>.max([],X)</code>: false.
</ul>
*/

namespace Assets.Code.Stdlib
{
    public class MaxStdLib: MinStdLib
    {
        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
                singleton = new MaxStdLib();
            return singleton;
        }

        override protected bool Compare(ITerm a, ITerm t)
        {
            return a.CompareTo(t) < 0;
        }
    }
}
