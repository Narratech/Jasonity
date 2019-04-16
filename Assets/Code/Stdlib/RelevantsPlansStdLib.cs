using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.relevant_plans</code></b>.
  <p>Description: gets all relevant plans for some triggering event. This
  internal action is used, for example, to answer "askHow" messages.
  <p>Parameters:<ul>
  <li>+ trigger (trigger): the triggering event, enclosed by { and }.</li>
  <li>- plans (list of plan terms): the list of plan terms corresponding to
  the code of the relevant plans.</li>
  <li><i>- labels</i> (list, optional): the list of labels of the plans.</li>
  </ul>
  <p>Example:<ul>
  <li> <code>.relevant_plans({+!go(X,Y)},LP)</code>: unifies LP with a list of
  all plans in the agent's plan library that are relevant for the triggering
  event <code>+!go(X,Y)</code>.</li>
  <li> <code>.relevant_plans({+!go(X,Y)},LP, LL)</code>: same as above but also
  unifies LL with a list of labels of plans in LP.</li>
  <li> <code>.relevant_plans({+!_},_,LL)</code>: gets the labels of all plans for achievement goals.</li>
  </ul>
 */

namespace Assets.Code.Stdlib
{
    public class RelevantsPlansStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 2;
        }
        override public int GetMaxArgs()
        {
            return 3;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            Trigger te = null;
            try {
                te = Trigger.TryToGetTrigger(args[0]);
            } catch (ParseException e) {}
            if (te == null)
                throw JasonityException.CreateWrongArgument(this,"first argument '"+args[0]+"' must follow the syntax of a trigger.");

            IListTerm labels = new ListTermImpl();
            IListTerm lt = new ListTermImpl();
            IListTerm last = lt;
            if (!te.GetLiteral().HasSource()) {
        	    // the ts.relevantPlans requires a source to work properly
        	    te.SetLiteral(te.GetLiteral().ForceFullLiteralImpl());
        	    te.GetLiteral().AddSource(new UnnamedVar());
            }
            List<Option> rp = ts.RelevantPlans(te);
            if (rp != null) {
                foreach (Option opt in rp) {
                    // remove sources (this IA is used for communication)
                    Plan np = (Plan)opt.GetPlan().Clone();
                    if (np.GetLabel() != null)
                        np.GetLabel().DelSources();
                    np.SetAsPlanTerm(true);
                    np.MakeVarsAnnon();
                    last = last.Append(np);
                    if (args.Length == 3)
                        labels.Add(np.GetLabel());
                }
            }

            bool ok = un.Unifies(lt, args[1]); // args[1] is a var;
            if (ok && args.Length == 3)
                ok = un.Unifies(labels, args[2]);

            return ok;
        }
    }
}
