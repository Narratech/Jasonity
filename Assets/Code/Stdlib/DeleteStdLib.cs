using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
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
    public class DeleteStdLib: InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new DeleteStdLib();
            return singleton;
        }

        public override int GetMinArgs()
        {
            return 3;
        }

        public override int GetMaxArgs()
        {
            return 4;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            if (args[0].IsNumeric())
            {
                int nextArg = 1;
                int start = (int)((args[0] as INumberTerm).Solve());
                int end = start + 1;
                if (args.Length == 4 && args[1].IsNumeric())
                {
                    nextArg = 2;
                    end = (int)((args[0] as INumberTerm).Solve());
                }
                if (args[nextArg].IsString())
                {
                    return un.Unifies(args[nextArg+1], DeleteFromString(start, end, args[nextArg] as IStringTerm));
                }
                else if (args[nextArg].IsList())
                {
                    return un.Unifies(args[nextArg + 1], DeleteFromList(start, end, args[nextArg] as IListTerm));
                }
            }
            if (args[0].IsString() && args[1].IsString())
            {
                return un.Unifies(args[2], DeleteFromString(args[0] as IStringTerm, args[1] as IStringTerm));
            }
            if (args[0].IsString())
            {
                return un.Unifies(args[2], DeleteFromString(args[0] as IStringTerm, new StringTermImpl(args[1].ToString())));
            }

            if (args[0].IsList())
            {
                return un.Unifies(args[2], DeleteFromList(args[0], args[1] as IListTerm, un.Clone()));
            }
            throw new JasonityException("Incorrect use of the internal action '.delete' (see documentation).");
        }

        IListTerm DeleteFromList(ITerm element, IListTerm l, Unifier un)
        {
            Unifier bak = un;
            IListTerm r = new ListTermImpl();
            IListTerm last = r;
            foreach (ITerm t in l)
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

        IListTerm DeleteFromList(int index, int end, IListTerm l)
        {
            IListTerm r = new ListTermImpl();
            IListTerm last = r;
            int i = 0;
            foreach (ITerm t in l)
            {
                if (i < index || i>= end)
                {
                    last = last.Append(t.Clone());
                }
                i++;
            }
            return r;
        }

        IStringTerm DeleteFromString(int index, int end, IStringTerm st)
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

        IStringTerm DeleteFromString(IStringTerm st1, IStringTerm st2)
        {
            try
            {
                string s1 = st1.GetString();
                string s2 = st2.GetString();
                return new StringTermImpl(s2.Replace(s1, " "));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return st1;
            }
        }
    }
}
