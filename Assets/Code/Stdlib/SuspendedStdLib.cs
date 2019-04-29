using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIMaAssets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.suspended(<i>G</i>, <i>R</i>)</code></b>.
  <p>Description: checks whether goal <i>G</i> belongs to a suspended intention. <i>R</i> (a String)
  unifies with the reason for the
  suspend (waiting action to be performed, .wait, ....).
  The literal <i>G</i>
  represents a suspended goal if there is a triggering event <code>+!G</code> in any plan within
  any intention in PI or PA.
  <p>Example:<ul>
  <li> <code>.suspended(go(1,3),R)</code>: true if <code>go(1,3)</code>
  is a suspended goal. <code>R</code> unifies with "act" if the reason for being suspended
  is an action waiting feedback from environment.
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class SuspendedStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 2;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[0].IsLiteral())
                throw JasonityException.CreateWrongArgument(this,"first argument must be a literal");
        }

        private static readonly ITerm aAct = new StringTermImpl("act");

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            Circumstance C = ts.GetCircumstance();
            Trigger teGoal = new Trigger(TEOperator.add, TEType.achieve, (Literal)args[0]);

            // search in PA
            foreach (ExecuteAction a in C.GetPendingActions().Values)
                if (a.GetIntention().HasTrigger(teGoal, un))
            return un.Unifies(args[1], aAct);

            // search in PI
            Dictionary<string, Intention> pi = C.GetPendingIntentions();
            foreach (string reason in pi.Keys)
                if (pi[reason].HasTrigger(teGoal, un))
                    return un.Unifies(args[1], new StringTermImpl(reason));

            return false;
        }
    }
}
