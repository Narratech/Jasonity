using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: broadcasts a message to all known agents.
 */
namespace Assets.Code.Stdlib
{
    public class Broadcast: DefaultInternalAction
    {
        public override int GetMinArgs()
        {
            return 2;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        public override void CheckArguments(ITerm[] args): base.CheckArguments(args)
        {
            if (!args[0].IsAtom())
            {
                throw new JasonException.CreateWrongArgument(this, "illocutionary force argument must be an atom");
            }
        }

        public override bool CanBeUsedInContext()
        {
            return false;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            ITerm ilf = args[0];
            ITerm pcnt = args[0];

            Message m = new Message(ilf.ToString(), ts.GetUserAgArch().GetAgName(), null, pcnt);
            ts.GetUserAgArch().Broadcast(m);
            return true;
        }
    }
}
