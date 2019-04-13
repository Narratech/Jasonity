using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Drop_All_Intentions:DefaultInternalAction
    {
        public int GetMinArgs()
        {
            return 0;
        }

        public int GetMaxArgs()
        {
            return 0;
        }

        public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            Circumstance C = ts.GetCircumstance();
            C.ClearRunnigIntentions();
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
            foreach (string ek in C.GetPendingEvents().KeySet())
            {
                Event e = C.GetPendingEvents().Get(ek);
                if (e.IsInternal())
                {
                    C.RemovePendingEvent(ek);
                }
            }

            At atia = ts.GetAgent().GetIA(At.atAtom) as At;
            atia.CancelAts();
            return true;
        }
    }
}
