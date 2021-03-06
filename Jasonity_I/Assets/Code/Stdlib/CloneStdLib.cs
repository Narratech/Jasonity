﻿using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using Assets.Code.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class CloneStdLib: InternalAction
    {
        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            string agName = ((IStringTerm)args[0]).GetString();
            IRuntimeServices services = ts.GetUserAgArch().GetRuntimeServices();
            services.Clone(ts.GetAgent(), ts.GetUserAgArch().GetAgArchClassesChain(), agName);

            return true;
        }
    }
}
