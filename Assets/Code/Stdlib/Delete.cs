using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    /*
     * Description: delete elements of strings or lists.
     */
    public class Delete: DefaultInternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new Delete();
            return singleton;
        }

        public int GetMinArgs()
        {
            return 3;
        }

        public int GetMaxArgs()
        {
            return 4;
        }

        public object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);

            if (args[0].IsNumeric())
            {
                int nextArg = 1;
                int start = (args[0] as NumberTerm).Solve() as int;
                int end = start + 1;
                if (args.Length == 4 && args[1].IsNumeric())
                {
                    nextArg = 2;
                    end = (args[0] as NumberTerm).Solve() as int;
                }
                if (args[nextArg].IsString())
                {
                    return un.Unifies(args[nextArg+1], DeleteFromString(start, end, args[nextArg] as StringTerm));
                }
                else if (args[nextArg].IsList())
                {
                    return un.Unifies(args[nextArg + 1], DeleteFromList(start, end, args[nextArg] as ListTerm));
                }
            }
            if (args[0].IsString() && args[1].IsString())
            {
                return un.Unifies(args[2], DeleteFromString(args[0] as StringTerm, args[1] as StringTerm));
            }
            if (args[0].IsString())
            {
                return un.Unifies(args[2], DeleteFromString(args[0] as StringTerm, new StringTermImpl(args[1].ToString())));
            }

            if (args[0].IsList())
            {
                return un.Unifies(args[2], DeleteFromList(args[0], args[1] as ListTerm, un.Clone()));
            }
            throw new JasonException("Incorrect use of the internal action '.delete' (see documentation).");
        }

        ListTerm DeleteFromList(Term element, ListTerm l, Unifier un)
        {
            Unifier bak = un;
            ListTerm r = new ListTermImpl();
            ListTerm last = r;
            foreach (Term t in l)
            {
                if (un.Unifies(element, t))
                {
                    un = bak.Clone();
                }
                else
                {
                    last = last.Append(t.Clone());
                }
            }
            return r;
        }

        ListTerm DeleteFromList(int index, int end, ListTerm l)
        {
            ListTerm r = new ListTermImpl();
            ListTerm last = r;
            int i = 0;
            foreach (Term t in l)
            {
                if (i < index || i>= end)
                {
                    last = last.Append(t.Clone());
                }
                i++;
            }
            return r;
        }

        StringTerm DeleteFromString(int index, int end, StringTerm st)
        {
            try
            {
                string s = st.GetString();
                return new StringTermImpl(s.Substring(0, index) + s.Substring(end));
            }
            catch (Exception e)
            {
                return st;
            }
        }

        StringTerm DeleteFromString(StringTerm st1, StringTerm st2)
        {
            try
            {
                string s1 = st1.GetString();
                string s2 = st2.GetString();
                return new StringTermImpl(s2.replaceAll(s1, " "));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return st1;
            }
        }
    }
}
