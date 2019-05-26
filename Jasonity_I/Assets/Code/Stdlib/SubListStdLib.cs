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
  <p>Internal action: <b><code>.sublist(<i>S</i>,<i>L</i>)</code></b>.
  <p>Description: checks if some list <i>S</i> is a sublist of list <i>L</i>. If
  <i>S</i> has free variables, this internal action backtracks all
  possible values for <i>S</i>. This is based on .prefix and .suffix (try prefixes first then prefixes of each suffix).
  <p>Parameters:<ul>
  <li>+/- sublist (list): the sublist to be checked.</li>
  <li>+ list (list): the list where the sublist is from.</li>
  </ul>
  <p>Examples:<ul>
  <li> <code>.sublist([a],[a,b,c])</code>: true.</li>
  <li> <code>.sublist([b],[a,b,c])</code>: true.</li>
  <li> <code>.sublist([c],[a,b,c])</code>: true.</li>
  <li> <code>.sublist([a,b],[a,b,c])</code>: true.</li>
  <li> <code>.sublist([b,c],[a,b,c])</code>: true.</li>
  <li> <code>.sublist([d],[a,b,c])</code>: false.</li>
  <li> <code>.sublist([a,c],[a,b,c])</code>: false.</li>
  <li> <code>.sublist(X,[a,b,c])</code>: unifies X with any sublist of the list, i.e., [a,b,c], [a,b], [a], [b,c], [b], [c], and [] in this order;
                                         note that this is not the order in which its usual implementation would return in logic programming (see note on .prefix).</li>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class SubListStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
                singleton = new SubListStdLib();
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

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ITerm sublist = args[0];

            return new SubListStdLibIterator<Unifier>(ts, un, args, sublist);
        }

        private class SubListStdLibIterator<T> : IEnumerator<Unifier>
        {
            private Reasoner ts;
            private Unifier un;
            private ITerm[] args;
            private ITerm sublist;
            private Unifier c; // the current response (which is an unifier)
            private IListTerm listOutter;
            private List<ITerm> list; // used in the inner loop, Java List is used for faster remove in the end
            private bool triedEmpty;
            public SubListStdLibIterator(Reasoner ts, Unifier un, ITerm[] args, ITerm sublist)
            {
                this.ts = ts;
                this.un = un;
                this.args = args;
                this.sublist = sublist;
                this.c = null;
                listOutter = (IListTerm)args[1];
                list = listOutter.GetAsList();
                triedEmpty = false;
            }

            //public bool HasNext()
            //{
            //    if (c == null) // the first call of hasNext should find the first response
            //        Find();
            //    return c != null;
            //}

            //public Unifier Next()
            //{
            //    if (c == null) Find();
            //    Unifier b = c;
            //    Find(); // find next response
            //    return b;
            //}

            void Find()
            {
                while (listOutter != null && listOutter.Count != 0)
                {
                    while (list.Count != 0)
                    {
                        IListTerm candidate = AsSyntax.AsSyntax.CreateList(list);
                        list.Remove(list.ElementAt(list.Count-1));
                        c = un.Clone();
                        if (c.UnifiesNoUndo(sublist, candidate))
                        {
                            return; // found another sublist, c is the current response
                        }
                    }
                    listOutter = listOutter.GetNext();
                    if (listOutter == null || listOutter.IsVar()) // the case of lists with tail
                        break;
                    list = listOutter.GetAsList();
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
                    if (c == null) Find();
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
                if (c == null) // the first call of hasNext should find the first response
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
