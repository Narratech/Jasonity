using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using Assets.Code.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: get the names of all agents in the system.
 */

namespace Assets.Code.stdlib
{
    public class AllNamesStdLib: InternalAction
    {
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
            RuntimeServices rs = ts.GetUserAgArch().GetRuntimeServices();
            IListTerm ln = new ListTermImpl();
            IListTerm tail = ln;
            foreach (string a in rs.GetAgentsNames())
            {
                tail = tail.Append(new Atom(a));
            }
            return un.Unifies(args[0], ln);
        }


    }
}
