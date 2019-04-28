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
  <p>Internal action: <b><code>.shuffle(List,Result)</code></b>.
  <p>Description: shuffle the elements of the <i>List</i> and unifies the result in <i>Var</i>.
  <p>Parameters:<ul>
  <li>+ input (list): the list to be shuffled<br/>
  <li>- result (list): the list with the elements shuffled.<br/>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class ShuffleStdLib:InternalAction
    {
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
            if (! (args[0].IsList()))
                throw JasonityException.CreateWrongArgument(this,"first argument must be a list");
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            List<ITerm> lt = ((IListTerm)args[0]).GetAsList();
            //Collections.shuffle(lt); //programarse un shuffler
            Random r = new Random();
            lt = lt.OrderBy(x => r.Next()).ToList();
            IListTerm ls = new ListTermImpl();

            foreach (ITerm i in lt)
            {
                ls.Add(i);
            }
            return un.Unifies(args[1], ls);
        }
    }
}
