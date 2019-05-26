using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class DropDesireStdLib:DropIntentionStdLib
    {
        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            base.Execute(ts, un, args);
            DropEvt(ts.GetCircumstance(), args[0] as Literal, un);
            return true;
        }

        public void DropEvt(Circumstance C, Literal l, Unifier un)
        {
            Trigger te = new Trigger(TEOperator.add, TEType.achieve, l);

            //search in E
            C.RemoveEvents(te, un);

            // search in PE (only the event need to be checked, the related intention is handled by dropInt)
            C.RemovePendingEvents(te, un);
        }
    }
}
