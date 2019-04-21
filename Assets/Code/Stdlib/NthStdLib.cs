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
<p>Internal action: <b><code>.nth</code></b>.
<p>Description: gets the nth term of a list.
<p>Parameters:<ul>
<li>-/+ index (integer): the position of the term (the first term is at position 0)<br/>
<li>+ list (list or string): the list/string where to get the term from.<br/>
<li>-/+ term (term): the term at position <i>index</i> in the <i>list</i>.<br/>
</ul>
<p>Examples:<ul>
<li> <code>.nth(0,[a,b,c],X)</code>: unifies <code>X</code> with <code>a</code>.
<li> <code>.nth(2,[a,b,c],X)</code>: unifies <code>X</code> with <code>c</code>.
<li> <code>.nth(2,"abc",X)</code>: unifies <code>X</code> with <code>c</code>.
<li> <code>.nth(0,[a,b,c],d)</code>: false.
<li> <code>.nth(0,[a,b,c],a)</code>: true.
<li> <code>.nth(5,[a,b,c],X)</code>: error.
<li> <code>.nth(X,[a,b,c,a,e],a)</code>: unifies <code>X</code> with <code>0</code> (and <code>3</code> if it backtracks).
</ul>
*/

namespace Assets.Code.Stdlib
{
    public class NthStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new NthStdLib();
            return singleton;
        }

        override public int GetMinArgs()
        {
            return 3;
        }
        override public int GetMaxArgs()
        {
            return 3;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[0].IsNumeric() && !args[0].IsVar()) {
                throw JasonityException.CreateWrongArgument(this,"the first argument should be numeric or a variable -- not '"+args[0]+"'.");
            }
            if (!args[1].IsList() && !args[1].IsString()) {
                throw JasonityException.CreateWrongArgument(this,"the second argument should be a list or string and not '"+args[1]+"'.");
            }
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);


            IListTerm list = null;
            if (args[1].IsList())
                list = (IListTerm)args[1];
            else if (args[1].IsString())
            {
                list = new ListTermImpl();
                foreach (byte b in ((IStringTerm)args[1]).GetString().GetBytes())
                {
                    list.Add(new StringTermImpl(new string(new byte[] { b })));
                }
            }

            if (args[0].IsNumeric())
            {
                int index = (int)((INumberTerm)args[0]).Solve();

                if (index < 0 || index >= list.Size())
                {
                    throw new JasonityException("nth: index " + index + " is out of bounds (" + list.Size() + ")");
                }

                return un.Unifies(args[2], list[index]);
            }

            if (args[0].IsVar())
            {
                IEnumerator<ITerm> ilist = list.ListTermIterator();
                //return all index for thirds arg
                return new NthStdLibIterator<Unifier>(ilist, un, args); 
            }
            return false;
        }

        private class NthStdLibIterator<T> : IEnumerator<Unifier>
        {
            int index = -1;
            Unifier c = default; // the current response (which is an unifier)
            IEnumerator<ITerm> ilist;
            Unifier un;
            ITerm[] args;

            public NthStdLibIterator(IEnumerator<ITerm> ilist, Unifier un, ITerm[] args)
            {
                this.ilist = ilist;
                this.args = args;
            }

            public bool HasNext()
            {
                if (c == null) // the first call of hasNext should find the first response
                    Find();
                return c != default;
            }

            public Unifier Next()
            {
                if (c == null)
                    Find(); // find next response
                Unifier b = c;
                Find();
                return b;
            }

            void Find()
            {
                while (ilist.MoveNext())
                {
                    index++;
                    ITerm candidate = ilist.Current;
                    c = un.Clone();
                    if (c.UnifiesNoUndo(args[2], candidate))
                    {
                        c.Unifies(args[0], AsSyntax.AsSyntax.CreateNumberTerm(index));
                        return; // found another
                    }
                }
                c = null;
            }

            public void Remove() { }

            public Unifier Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
