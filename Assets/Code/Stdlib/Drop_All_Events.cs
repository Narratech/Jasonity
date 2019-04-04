using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Drop_All_Events:DefaultInternalAction
    {
        public int GetMinArgs()
        {
            return 0;
        }
        public int GetMaxArgs()
        {
            return 0;
        }

        public object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            ts.GetC().ClearEvents();
            ts.GetC().ClearPendingEvents();
            return true;
        }
    }
}
