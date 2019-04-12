using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: checks whether a list has at least one term.
 */
namespace Assets.Code.Stdlib
{
    public class Empty:DefaultInternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
                singleton = new Empty();
            return singleton;
        }

        public int GetMinArgs()
        {
            return 1;
        }

        public int GetMaxArgs()
        {
            return 1;
        }

        public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ITerm l1 = args[0];
            if (l1.IsList())
            {
                IListTerm lt = l1 as IListTerm;
                return lt.IsEmpty();
            }
            else if (l1.IsString())
            {
                IStringTerm st = l1 as IStringTerm;
                return st.GetString().IsEmpty();
            }
            return false;
        }
    }
}
