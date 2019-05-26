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
  <p>Internal action: <b><code>.suffix(<i>S</i>,<i>L</i>)</code></b>.
  <p>Description: checks if some list <i>S</i> is a suffix of list <i>L</i>. If
  <i>S</i> has free variables, this internal action backtracks all
  possible values for <i>S</i>.
  <p>Parameters:<ul>
  <li>+/- suffix (list): the suffix to be checked.</li>
  <li>+ list (list): the list where the suffix is from.</li>
  </ul>
  <p>Examples:<ul>
  <li> <code>.suffix([c],[a,b,c])</code>: true.</li>
  <li> <code>.suffix([a,b],[a,b,c])</code>: false.</li>
  <li> <code>.suffix(X,[a,b,c])</code>: unifies X with any suffix of the list, i.e., [a,b,c], [b,c], [c], and [] in this order.</li>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class SuffixStdLib: InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new SuffixStdLib();
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
            IEnumerator<IListTerm> list = ((IListTerm)args[1]).ListTermIterator();

            return new SuffixStdLibIterator<Unifier>(reasoner, un, args, sublist, list);
        }

        private class SuffixStdLibIterator<T> : IEnumerator<Unifier>
        {
            Unifier c; // the current response (which is an unifier)
            private Reasoner reasoner;
            private Unifier un;
            private ITerm[] args;
            private ITerm sublist;
            private IEnumerator<IListTerm> list;

            public SuffixStdLibIterator(Reasoner reasoner, Unifier un, ITerm[] args, ITerm sublist, IEnumerator<IListTerm> list)
            {
                this.reasoner = reasoner;
                this.un = un;
                this.args = args;
                this.sublist = sublist;
                this.list = list;
                c = null;
            }

            //public bool HasNext()
            //{
            //    if (c == null) // the first call of hasNext should find the first response
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
                while (list.MoveNext())
                {
                    IListTerm l = list.Current;
                    if (l.IsVar()) // the case of the tail of the list
                        break;
                    c = un.Clone();
                    if (c.UnifiesNoUndo(sublist, AsSyntax.AsSyntax.CreateList(l)))
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
