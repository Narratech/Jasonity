using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Beliefs;
using BDIManager.Desires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: adds plan(s) to the agent's plan library.
 */
namespace Assets.Code.Stdlib
{
    public class AddPlanStdLib: InternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 3;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            ITerm source = BeliefBase.ASelf;
            if (args.Length > 1)
            {
                source = args[1];
            }

            bool before = false;
            if (args.Length > 2)
            {
                before = args[2].ToString().Equals("begin");
            }

            if (args[0].IsList())
            {
                foreach (ITerm t in (IListTerm)args[0])
                {
                    ts.GetAgent().GetPL().Add(Transform2Plan(t), source, before);
                }
            }
            else
            {
                ts.GetAgent().GetPL().Add(Transform2Plan(args[0]), source, before);
            }

            if (ts.GetAgent().GetPL().HasMetaEventPlans())
            {
                ts.AddDesireListener(new DesireStdlib(ts));
            }
            return true;
        }

        private Plan Transform2Plan(ITerm t)
        {
            Plan p = null;
            if (t.IsString())
            {
                string sPlan = ((IStringTerm)t).GetString();
                // remove quotes \" -> "
                StringBuilder sTemp = new StringBuilder();
                for (int c = 0; c < sPlan.Length; c++)
                {
                    if (sPlan.ElementAt(c) != '\\')
                    {
                        sTemp.Append(sPlan.ElementAt(c));
                    }
                }
                sPlan = sTemp.ToString();
                p = AsSyntax.AsSyntax.ParsePlan(sPlan);
            }
            else if (t.GetType() == typeof(Plan))
            {
                p = (Plan)t;
            } else {
                throw JasonityException.CreateWrongArgument(this, "The term '" + t + "' (" + t.GetType().Name + ") can not be used as a plan for .add_plan.");
            }
            if (p.GetLabel() != null && p.GetLabel().GetFunctor().StartsWith("l__"))
            {
                // if the label is automatic label, remove it
                p.DelLabel();
            }
            return p;
        }
    }
}
