using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.prefix(<i>P</i>,<i>L</i>)</code></b>.
  <p>Description: checks if some list <i>P</i> is a prefix of list <i>L</i>. If
  <i>P</i> has free variables, this internal action backtracks all
  possible values for <i>P</i>.
  <p>Parameters:<ul>
  <li>+/- prefix (list): the prefix to be checked.</li>
  <li>+ list (list): the list where the prefix is from.</li>
  </ul>
  <p>Examples:<ul>
  <li> <code>.prefix([a],[a,b,c])</code>: true.</li>
  <li> <code>.prefix([a,b],[a,b,c])</code>: true.</li>
  <li> <code>.prefix([b,c],[a,b,c])</code>: false.</li>
  <li> <code>.prefix(X,[a,b,c])</code>: 
    unifies X with any prefix of the list, i.e., [a,b,c], [a,b], [a], and [] in this order;
    note that this is different from what its usual implementation in logic programming would result,
    where the various prefixes are returned in increasing lengths instead.</li>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class PrefixStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
                singleton = new PrefixStdLib();
            return singleton;
        }

        // Needs exactly 2 arguments
        override public int GetMinArgs()
        {
            return 2;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        // improve the check of the arguments to also check the type of the arguments
        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[0].IsList() && !args[0].IsVar())
                throw JasonityException.CreateWrongArgument(this,"first argument must be a list or a variable");
            if (!args[1].IsList())
                throw JasonityException.CreateWrongArgument(this,"second argument must be a list");
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            // execute the internal action

            ITerm sublist = args[0];
            List<ITerm> list = ((IListTerm)args[1]).GetAsList(); // use a Java List for better performance in remove last

            return new PrefixStdLibIterator<Unifier>(un, list, sublist);
        }

        private class PrefixStdLibIterator<T> : IEnumerator<Unifier>
        {
            Unifier un, c; // the current response (which is an unifier)
            bool triedEmpty;
            List<ITerm> list;
            ITerm sublist;

            public PrefixStdLibIterator(Unifier un, List<ITerm> list, ITerm sublist)
            {
                c = null;
                this.sublist = sublist;
                this.un = un;
                triedEmpty = false;
                this.list = list;
            }

            //public bool HasNext()
            //{
            //    if (c == null)
            //        Find();
            //    return c != null;
            //}

            //public Unifier Next()
            //{
            //    if (c == null)
            //        Find();
            //    Unifier b = c;
            //    Find(); // find next response
            //    return b;
            //}

            void Find()
            {
                while (!(list.Count == 0))
                {
                    IListTerm candidate = AsSyntax.AsSyntax.CreateList(list);
                    list.Remove(list.ElementAt(list.Count - 1));
                    c = un.Clone();
                    if (c.UnifiesNoUndo(sublist, candidate))
                    {
                        return; // found another sublist, c is the current response
                    }
                }
                if (!triedEmpty)
                {
                    triedEmpty = true;
                    c = un.Clone();
                    if (c.UnifiesNoUndo(sublist, AsSyntax.AsSyntax.CreateList()))
                    {
                        return; // found another sublist, c is the current response
                    }
                }
                c = null; // no more sublists found
            }

            //public void Remove() { }

            public Unifier Current
            {
                get
                {
                    if (c == null)
                        Find();
                    Unifier b = c;
                    Find(); // find next response
                    return b;
                }
            }

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                if (c == null)
                    Find();
                return c != null;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
