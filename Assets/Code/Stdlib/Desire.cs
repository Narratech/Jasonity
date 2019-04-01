using Assets.Code.Logic;
using Assets.Code.Logic.AsSyntax;
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

        public object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            checkArguments(args);
            return AllDesires(ts.GetC(), args[0] as Literal, args.Length == 2 ? args[1] : null, un);
        }

        public static IEnumerator<Unifier> AllDesires(Circumstance C, Literal l, Term intAsTerm, Unifier un)
        {
            const Trigger teFroml = new Trigger(TeOperator.Add, TeType.Achive, l);

            return new IEnumerator<Unifier>()
            {

            }
        }

    }
}
