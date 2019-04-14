using Assets.Code.Agent;
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
    public class EmptyStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new EmptyStdLib();
            return singleton;
        }

        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 1;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ITerm l1 = args[0];
            if (l1.IsList())
            {
                IListTerm lt = l1 as IListTerm;
                return lt.Count == 0;
            }
            else if (l1.IsString())
            {
                IStringTerm st = l1 as IStringTerm;
                return st.GetString().Length == 0;
            }
            return false;
        }
    }
}
