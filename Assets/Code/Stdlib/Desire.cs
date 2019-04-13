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
    public class Desire : Intend
    {
        enum Step { selEvt, evt, useIntends, end }

        public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return AllDesires(ts.GetCircumstance(), args[0] as Literal, args.Length == 2 ? args[1] : null, un);
        }

        public static IEnumerator<Unifier> AllDesires(Circumstance C, Literal l, ITerm intAsTerm, Unifier un)
        {
            const Trigger teFroml = new Trigger(TEOperator.add, TEType.achieve, l);

            return new IEnumerator<Unifier>()
            {

            }
        }

    }
}
