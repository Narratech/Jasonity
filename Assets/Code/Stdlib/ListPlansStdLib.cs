using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
<p>Internal action: <b><code>.list_plans</code></b>.
<p>Description: prints out the plans in the plan library.
<p>Parameter:<ul>
<li>+ trigger (trigger term -- optional): list only plan that unifies this parameter as trigger event.<br/>
</ul>
<p>Examples:<ul>
<li> <code>.list_plans</code>: list all agent's plans
<li> <code>.list_plans({ +g(_) })</code>: list agent's plans that unifies with +g(_) 
</ul>
*/
namespace Assets.Code.Stdlib
{
    public class ListPlansStdLib:InternalAction
    {
        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            if (args.Length == 1 && (args[0].GetType() == typeof(Trigger))) {
                Trigger te = Trigger.TryToGetTrigger(args[0]);
                if (!te.GetLiteral().HasSource())
                {
                    te.GetLiteral().AddSource(new UnnamedVar());
                }

                foreach (Plan p in reasoner.GetAgent().GetPL())
                {
                    if (te == null || new Unifier().Unifies(p.GetTrigger(), te))
                    {
                        //reasoner.GetLogger().Info(p.ToString());
                    }
                }
            } else {
                //reasoner.GetLogger().Info(reasoner.GetAgent().GetPL().GetAsTxt(false));
            }
            return true;
        }
    }
}
