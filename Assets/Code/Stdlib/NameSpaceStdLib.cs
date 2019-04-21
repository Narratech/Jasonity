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

namespace Assets.Code.Stdlib
{
    public class NameSpaceStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 1;
        }
        override public int GetMaxArgs()
        {
            return 1;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[0].IsAtom() & !args[0].IsVar())
                throw JasonityException.CreateWrongArgument(this,"first argument must be an atom or variable.");
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            if (!args[0].IsVar())
            {
                return ts.GetAgent().GetBB().GetNameSpaces().Contains(args[0]);
            }
            else
            {
                return new NameSpaceStdLibIterator<Unifier>(ts, un, args);
            }
        }

        private class NameSpaceStdLibIterator<T> : IEnumerator<Unifier>
        {
            Reasoner ts;
            IEnumerator<Atom> i;
            Unifier n, un;
            ITerm[] args;

            public NameSpaceStdLibIterator(Reasoner ts, Unifier un, ITerm[] args)
            {
                this.ts = ts;
                i = this.ts.GetAgent().GetBB().GetNameSpaces().GetEnumerator();
                n = default;
                this.un = un;
                this.args = args;
            }

            //PERO QUÉ MIERRRRRDAS ES ESTO?!?!
            /*
            {
                next(); // consume the first (and set first n value, i.e. the first solution)
            } 
            */

            public bool HasNext()
            {
                return n != default;
            }

            public Unifier Next()
            {
                Unifier c = n;

                n = un.Clone();
                if (i.MoveNext())
                {
                    if (!n.UnifiesNoUndo(args[0], i.Current))
                        Next();
                }
                else
                {
                    n = default;
                }
                return c;
            }

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

        public void Remove() { }
    }
}
