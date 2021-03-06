﻿using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class DropAllIntentionsStdLib:InternalAction
    {
        public override int GetMinArgs()
        {
            return 0;
        }

        public override int GetMaxArgs()
        {
            return 0;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            Circumstance C = ts.GetCircumstance();
            C.ClearRunningIntentions();
            C.ClearPendingIntentions();
            C.ClearPendingActions();

            // drop intentions in E
            IEnumerator<Event> ie = C.GetEventsPlusAtomic();
            while (ie.Current != null)
            {
                Event e = ie.Current;
                if (e.IsInternal())
                {
                    C.RemoveEvent(e);
                }
            }

            //drop intentions in PE
            foreach (string ek in C.GetPendingEvents().Keys)
            {
                Event e = C.GetPendingEvents()[ek];
                if (e.IsInternal())
                {
                    C.RemovePendingEvent(ek);
                }
            }

            AtStdLib atia = ts.GetAgent().GetIA(AtStdLib.atAtom) as AtStdLib;
            atia.CancelAts();
            return true;
        }
    }
}
