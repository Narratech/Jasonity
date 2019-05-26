using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Code.Stdlib
{
    public class IntendStdLib : InternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        protected override void CheckArguments(ITerm[] args) 
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[0].IsLiteral() && !args[0].IsVar())
                throw JasonityException.CreateWrongArgument(this,"first argument must be a literal or variable");
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return AllIntentions(ts.GetCircumstance(), (Literal) args[0], args.Length == 2 ? args[1] : null, un);
        }

        /**
         * returns all unifications for intentions with some goal
         */
        public static IEnumerator<Unifier> AllIntentions(Circumstance C, Literal l, ITerm intAsTerm, Unifier un)
        {
            Trigger g = new Trigger(TEOperator.add, TEType.achieve, l);

            return new EnumeratorImpl(C, un, g, intAsTerm);
        }

        private class EnumeratorImpl : IEnumerator<Unifier>
        {
            Unifier un;
            Circumstance C;
            Unifier solution = null; // the current response (which is an unifier)
            Intention curInt = null;
            IEnumerator<Intention> intIterator;
            Trigger g;
            ITerm intAsTerm;

            public Unifier Current
            {
                get
                {
                    if (solution == null) Find();
                    Unifier b = solution;
                    Find(); // find next response
                    return b;
                }
            }

            object IEnumerator.Current => throw new System.NotImplementedException();

            public EnumeratorImpl(Circumstance C, Unifier un, Trigger g, ITerm intAsTerm)
            {
                this.g = g;
                this.un = un;
                this.C = C;
                this.intAsTerm = intAsTerm;
                intIterator = C.GetAllIntentions();
            } 

            public bool MoveNext()
            {
                return solution != null;
            }

            //public Unifier Next()
            //{
            //    if (solution == null) Find();
            //    Unifier b = solution;
            //    Find(); // find next response
            //    return b;
            //}

            bool IsSolution()
            {
                solution = un.Clone();
                if (curInt.HasTrigger(g, solution))
                {
                    if (intAsTerm != null)
                    {
                        return solution.Unifies(intAsTerm, curInt.GetAsTerm());
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }

            void Find()
            {
                while (intIterator.MoveNext())
                {
                    curInt = intIterator.Current;
                    if (IsSolution())
                        return;
                }
                solution = null; // nothing found
            }

            public void Reset()
            {
                throw new System.NotImplementedException();
            }

            public void Dispose()
            {
                
            }
        }   
    }
}