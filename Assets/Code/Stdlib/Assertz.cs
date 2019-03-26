using Assets.Code.Logic;
using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Assertz: DefaultInternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 1;
        }

        protected override void CheckArguments(Term[] args): base.CheckArguments(args)
        {
            if (!args[0].IsLiteral())
            {
                if (!args[0].IsGround() && !args[0].IsRule())
                {
                    throw new JasonExecption.CreateWrongArguments(this, "first argument must be a ground literal (or rule).");
                }
            }
        }

        public  override object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);

            List<Literal>[] result = ts.GetAg().Brf((Literal)args[0], null, null, true);
            if (result != null)
            {
                ts.UpdateEvents(result, null);
            }
            return true;
        }
    }
}
