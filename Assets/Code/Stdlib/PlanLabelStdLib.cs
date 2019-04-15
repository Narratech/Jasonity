using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
  <p>Internal action: <b><code>.plan_label(<i>P</i>,<i>L</i>)</code></b>.
  <p>Description: unifies <i>P</i> with a <i>plan term</i> representing the plan
  labeled with the term <i>L</i> within the agent's plan library.
  <p>Parameters:<ul>
  <li>- plan (plan term): the term representing the plan, it is
  a plan enclosed by { and }
  (e.g. <code>{+!g : vl(X) <- .print(X)}</code>).<br/>
  <li>+ label (structure): the label of that plan.<br/>
  </ul>
  <p>Example:<ul>
  <li> <code>.plan_label(P,p1)</code>: unifies P with the term
  representation of the plan labeled <code>p1</code>.</li>
  </ul>
 */

namespace Assets.Code.Stdlib
{
    public class PlanLabelStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new PlanLabelStdLib();
            return singleton;
        }

        override public int GetMinArgs()
        {
            return 2;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            ITerm label = args[1];
            Plan p;
            if (label.IsLiteral())
                p = ts.GetAgent().GetPL().Get((Literal) label);
            else
                p = ts.GetAgent().GetPL().Get( new Atom(label.ToString()));

            if (p != null) {
                p = (Plan) p.Clone();
                p.GetLabel().DelSources();
                p.SetAsPlanTerm(true);
                p.MakeVarsAnnon();
                //String ps = p.toASString().replaceAll("\"", "\\\\\"");
                //return un.unifies(new StringTermImpl(ps), args[0]);
                return un.Unifies(p, args[0]);
            } else {
                return false;
            }
        }
    }
}
