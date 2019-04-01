using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
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

        public object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            Term l1 = args[0];
            if (l1.IsList())
            {
                ListTerm lt = l1 as ListTerm;
                return lt.isEmpty();
            }
            else if (l1.IsString())
            {
                StringTerm st = l1 as StringTerm;
                return st.GetString().IsEmpty();
            }
            return false;
        }
    }
}
