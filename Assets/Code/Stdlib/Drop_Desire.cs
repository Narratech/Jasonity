using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Drop_Desire:Drop_Intention
    {
        public object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            base.Execute(ts, un, args);
            DropEvt(ts.GetC(), args[0] as Literal, un);
            return true;
        }

        public void DropEvt(Circumstance C, Literal l, Unifier un)
        {
            Trigger te = new Trigger(TEOperator.Add(), TEType.Achive(), l);

            //search in E
            C.RemoveEvents(te, un);

            // search in PE (only the event need to be checked, the related intention is handled by dropInt)
            C.RemovePendingEvents(te, un);
        }
    }
}
