using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
<p>Internal action: <b><code>.list_rules</code></b>.
<p>Description: prints out the rules in the belief base.
<p>Example:<ul>
<li> <code>.list_rules</code>: list rules in the agent's belief base
</ul>
*/
namespace Assets.Code.Stdlib
{
    public class ListRulesStdLib:InternalAction
    {
        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            foreach (Literal b in reasoner.GetAgent().GetBB())
            {
                reasoner.GetLogger().Info(ToString());
            }
            return true;
        }
    }
}
