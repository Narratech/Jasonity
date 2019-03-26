using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
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
    public class All_names: DefaultInternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 1;
        }

        public override object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            RuntimeServices rs = ts.GetUserAgArch().GetRuntimeServices();
            ListTerm ln = new ListTermImpl();
            ListTerm tail = ln;
            foreach (string a in rs.GetAgentsNames())
            {
                tail = tail.Append(new Atom(a));
            }
            return un.Unifies(args[0], ln);
        }


    }
}
