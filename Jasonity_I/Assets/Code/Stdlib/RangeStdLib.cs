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
  <p>Internal action: <b><code>.range(<i>Var</i>,<i>Start</i>,<i>End</i>, <i>Step</i>)</code></b>.
  <p>Description: backtrack all values for <i>Var</i> starting at <i>Start</i>
  and finishing at <i>End</i> by increments of <i>Step</i> (default step value is 1).
  <p>Parameters:<ul>
  <li>+/- var (Variable): the variable that unifies with all values.</li>
  <li>+ start (number): initial value.</li>
  <li>+ end (number): last value.</li>
  <li>+ end (number -- optional): step.</li>
  </ul>
  <p>Examples:<ul>
  <li> <code>.range(3,1,5)</code>: true.</li>
  <li> <code>.range(6,1,5)</code>: false.</li>
  <li> <code>.range(X,1,5)</code>: unifies X with 1, 2, 3, 4, and 5.</li>
  <li> <code>.range(X,1,11,2)</code>: unifies X with 2, 4, 6, 8, and 10.</li>
  <li> <code>.range(X,5,1,-1)</code>: unifies X with 5, 4, 3, 2, and 1.</li>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class RangeStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new RangeStdLib();
            return singleton;
        }

        override public int GetMinArgs()
        {
            return 3;
        }
        override public int GetMaxArgs()
        {
            return 4;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[1].IsNumeric())
                throw JasonityException.CreateWrongArgument(this,"second parameter ('" + args[1] + "') must be a number!");
            if (!args[2].IsNumeric())
                throw JasonityException.CreateWrongArgument(this,"third parameter ('" + args[2] + "') must be a number!");
            if (args.Length == 4 && !args[3].IsNumeric())
                throw JasonityException.CreateWrongArgument(this,"fourth parameter ('" + args[3] + "') must be a number!");
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            int start = (int)((INumberTerm)args[1]).Solve();
            int end = (int)((INumberTerm)args[2]).Solve();
            int step;
            if (args.Length == 4)
                step = (int)((INumberTerm)args[3]).Solve();
            else
                step = 1;
            if (!args[0].IsVar())
            {
                // first arg is not a var
                int vl = (int)((INumberTerm)args[0]).Solve();
                return vl >= start && vl <= end;
            }
            else
            {
                // first arg is a var, backtrack
                ITerm var = args[0];
                return new RangeStdLibIterator<Unifier>(un, start, step, end, var);
            }
        }

        private class RangeStdLibIterator<T> : IEnumerator<Unifier>
        {
            int vl;
            Unifier un;
            int start, step, end;
            ITerm var;

            public RangeStdLibIterator(Unifier un, int start, int step, int end, ITerm var)
            {
                this.un = un;
                this.step = step;
                this.start = start;
                this.end = end;
                this.var = var;
            }

            //public bool HasNext()
            //{
            //    if (step > 0)
            //        return vl + step <= end;
            //    else
            //        return vl + step >= end;
            //}

            //public Unifier Next()
            //{
            //    vl += step;
            //    Unifier c = un.Clone();
            //    c.UnifiesNoUndo(var, new NumberTermImpl(vl));
            //    return c;
            //}

            //public void Remove() { }

            public Unifier Current
            {
                get
                {
                    vl += step;
                    Unifier c = un.Clone();
                    c.UnifiesNoUndo(var, new NumberTermImpl(vl));
                    return c;
                }
            }

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                if (step > 0)
                    return vl + step <= end;
                else
                    return vl + step >= end;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
