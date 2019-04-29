using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using BDIManager.Beliefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
  <p>Internal action: <b><code>.remove_plan</code></b>.
  <p>Description: removes plans from the agent's plan library.
  <p>Parameters:<ul>
  <li>+ label(s) (structure or list of structures): the label of the
  plan to be removed. If this parameter is a list of labels, all plans
  of this list are removed.</li>
  <li><i>+ source</i> (atom [optional]): the source of the
  plan to be removed. The default value is <code>self</code>.</li>
  </ul>
  <p>Examples:<ul>
  <li> <code>.remove_plan(l1)</code>: removes the plan identified by
  label <code>l1[source(self)]</code>.</li>
  <li> <code>.remove_plan(l1,bob)</code>: removes the plan identified
  by label <code>l1[source(bob)]</code>. Note that a plan with a
  source like that was probably added to the plan library by a tellHow
  message.</li>
  <li> <code>.remove_plan([l1,l2,l3])</code>: removes the plans identified
  by labels <code>l1[source(self)]</code>, <code>l2[source(self)]</code>, and
  <code>l3[source(self)]</code>.</li>
  <li> <code>.remove_plan([l1,l2,l3],bob)</code>: removes the plans identified
  by labels <code>l1[source(bob)]</code>, <code>l2[source(bob)]</code>, and
  <code>l3[source(bob)]</code>.</li>
  <li> <code>.relevant_plans({ +!g }, _, L); .remove_plan(LL)</code>:
  removes all plans with trigger event <code>+!g</code>.</li>
  </ul>
 */

namespace Assets.Code.Stdlib
{
    public class RemovePlanStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 1;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ITerm label = args[0];

            ITerm source = BeliefBase.ASelf;
            if (args.Length > 1) {
                source = (Atom) args[1];
            }
            if (label.IsList()) { // arg[0] is a list
                foreach (ITerm t in (IListTerm) args[0]) {
                    //r = r && ts.getAg().getPL().remove((Atom)t, source);
                    ts.GetAgent().GetPL().Remove(FixLabel(t), source);
                }
            }
            else { // args[0] is a plan label
                ts.GetAgent().GetPL().Remove(FixLabel(label), source);
            }
            return true;
        }

        protected Literal FixLabel(ITerm label)
        {
    	if (label.IsString() && ((IStringTerm) label).GetString().StartsWith("@")) {
    		// as used in the book
    		label = AsSyntax.AsSyntax.ParseTerm(((IStringTerm) label).GetString().Substring(1));
    	}    	
    	    return (Literal) label;
        }
    }
}
