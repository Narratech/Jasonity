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
  <p>Internal action: <b><code>.random(<i>N</i>)</code></b>.
  <p>Description: unifies <i>N</i> with a random number between 0 and 1.
  <p>Parameter:<ul>
  <li>- value (number): the variable to receive the random value<br/>
  <li><i>+ quantity of random numbers</i> (number, optional): default value is 1, value = 0 means that an infinity number of random numbers will be produced (this is useful for some backtrack circumstances).</li>
  </ul>
  <p>Example:<ul>
  <li><code>.random(X)</code>: unifies X with one random number between 0 and 1.</li>
  <li><code>.random(X, 5)</code>: unifies X with a random number between 0 and 1, and backtracks 5 times. For example: .findall(X, .random(X,5), L) will produce a list of 5 random numbers.</li>
  <li><code>.random(X, 0)</code>: unifies X with a random number between 0 and 1, and backtracks infinitely.</li>
  </ul>
  @see jason.functions.Random function version
*/

namespace Assets.Code.Stdlib
{
    public class RandomStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new RandomStdLib();
            return singleton;
        }

        private Random random = new Random();

        override public int GetMinArgs()
        {
            return 1;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[0].IsVar())
                throw JasonityException.CreateWrongArgument(this,"first argument must be a variable.");
            if (args.Length == 2 && !args[1].IsNumeric())
                throw JasonityException.CreateWrongArgument(this,"second argument must be a number.");
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            if (args.Length == 1) {
                return un.Unifies(args[0], new NumberTermImpl(random.NextDouble()));
            }
            else {
                int max = (int)((INumberTerm)args[1]).Solve();

                return new RandomStdLibIterator<Unifier>(max, ts, un, random, args);
            }
        }

        private class RandomStdLibIterator<T> : IEnumerator<Unifier>
        {
            int n, max;
            Reasoner ts;
            Unifier un;
            Random random;
            ITerm[] args;

            public RandomStdLibIterator(int max, Reasoner ts, Unifier un, Random random, ITerm[] args)
            {
                n = 0;
                this.max = max;
                this.ts = ts;
                this.un = un;
                this.random = random;
                this.args = args;
            }

            // we always have a next random number
            //public bool HasNext()
            //{
            //    return (n < max || max == 0) && ts.GetUserAgArch().IsRunning();
            //}

            public Unifier Next()
            {
                Unifier c = un.Clone();
                c.Unifies(args[0], new NumberTermImpl(random.NextDouble()));
                n++;
                return c;
            }

            //public void Remove() { }

            public Unifier Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            {
                
            }

            // we always have a next random number
            public bool MoveNext()
            {
                return (n < max || max == 0) && ts.GetUserAgArch().IsRunning();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
