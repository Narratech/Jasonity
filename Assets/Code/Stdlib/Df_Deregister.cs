﻿using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Df_Deregister: Df_Register
    {
        private static InternalAction singleton = null;
        public new static InternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new Df_Deregister();
            }
            return singleton;
        }

        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ts.GetUserAgArch().GetRuntimeServices().DfDeRegister(ts.GetUserAgArch().GetAgentName(), GetService(args), GetType(args));
            return true;
        }
    }
}
