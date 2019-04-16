using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.substring</code></b>.
  <p>Description: checks if a string is sub-string of another
    string. The arguments can be other kinds of terms, in which case
    the toString() of the term is used. If "position" is a
    free variable, the internal action backtracks all possible values
    for the positions where the sub-string occurs in the string.
  <p>Parameters:<ul>
  <li>+ substring (any term).<br/>
  <li>+/- string (any term).<br/>
  <li>+/- position (optional -- integer): the initial position of
  the string where the sub-string occurs.
  <li>+/- position (optional -- integer): the last position of
  the string where the sub-string occurs.
  </ul>
  <p>Examples:<ul>
  <li> <code>.substring("b","aaa")</code>: false.
  <li> <code>.substring("b","aaa",X)</code>: false.
  <li> <code>.substring("a","bbacc")</code>: true.
  <li> <code>.substring("a","abbacca",X)</code>: true and <code>X</code> unifies with 0, 3, and 6.
  <li> <code>.substring("a","bbacc",0)</code>: false.
  <li> <code>.substring(a(10),b(t1,a(10)),X)</code>: true and <code>X</code> unifies with 5.
  <li> <code>.substring(a(10),b("t1,a(10),kk"),X)</code>: true and <code>X</code> unifies with 6.
  <li> <code>.substring(a(10,20),R,5)</code>: true and <code>R</code> unifies with "20)".
  <li> <code>.substring(a(10,20),R,5,7)</code>: true and <code>R</code> unifies with "20".
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class SubStringStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
                singleton = new SubStringStdLib();
            return singleton;
        }

        override public int GetMinArgs()
        {
            return 2;
        }
        override public int GetMaxArgs()
        {
            return 4;
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            string s0;
            if (args[0].IsString())
                s0 = ((IStringTerm)args[0]).GetString();
            else
                s0 = args[0].ToString();

            string s1;
            if (args[1].IsString())
                s1 = ((IStringTerm)args[1]).GetString();
            else
                s1 = args[1].ToString();

            if (args.Length == 2)
            {
                // no backtracking utilisation
                return s1.IndexOf(s0) >= 0;
            }
            else if (args[2].IsGround() && args[2].IsNumeric() && args[1].IsVar())
            {
                // no backtracking utilisation
                // unifies the var with the substring
                int start = (int)((INumberTerm)(args[2])).Solve();
                int end = s0.Length;
                if (args.Length == 4 && args[3].IsNumeric())
                    end = (int)((INumberTerm)(args[3])).Solve();
                return un.Unifies(args[1], new StringTermImpl(s0.Substring(start, end)));
            }
            else
            {
                // backtrack version: unifies in the third argument all possible positions of s0 in s1
                return new SubStringStdLibIterator<Unifier>(reasoner, un, args, s1, s0);
            }
        }

        private class SubStringStdLibIterator<T> : IEnumerator<Unifier>
        {
            Unifier c; // the current response (which is an unifier)
            int pos; // current position in s1
            private Reasoner reasoner;
            private Unifier un;
            private ITerm[] args;
            string s1, s0;

            public SubStringStdLibIterator(Reasoner reasoner, Unifier un, ITerm[] args, string s1, string s0)
            {
                this.reasoner = reasoner;
                this.un = un;
                this.args = args;
                c = default;
                pos = 0;
                this.s1 = s1;
                this.s0 = s0;                    
            }

            public bool HasNext()
            {
                if (c == null) // the first call of hasNext should find the first response
                    Find();
                return c != null;
            }

            public Unifier Next()
            {
                if (c == null) Find();
                    Unifier b = c;
                Find(); // find next response
                return b;
            }

            void Find()
            {
                if (pos < s1.Length)
                {
                    pos = s1.IndexOf(s0, pos);
                    if (pos >= 0)
                    {
                        c = (Unifier)un.Clone();
                        c.UnifiesNoUndo(args[2], new NumberTermImpl(pos));
                        pos++;
                        return;
                    }
                    pos = s1.Length; // to stop searching
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
