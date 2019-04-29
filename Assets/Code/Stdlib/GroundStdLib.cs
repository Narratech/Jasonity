using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class GroundStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new GroundStdLib();
            return singleton;
        }

        override public int GetMinArgs()
        {
            return 1;
        }

        override public int GetMaxArgs()
        {
            return 1;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return args[0].IsGround();
        }
}
}
