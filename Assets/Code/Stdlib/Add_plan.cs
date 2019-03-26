using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
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
    public class Add_plan: DefaultInternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 3;
        }

        public override object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);

            Term source = BeliefBase.ASelf;
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
                foreach (Term t in (ListTerm)args[0])
                {
                    ts.GetAg().GetPL().Add(Transform2Plan(t), source, before);
                }
            }
            else
            {
                ts.GetAg().GetPL().Add(Transform2Plan(args[0]), source, before);
            }

            if (ts.GetAg().GetPL().hasMetaEventPlans())
            {
                ts.AddGoalListener(new GoalListenerForMetaEvents(ts));
            }
            return true;
        }
    }
}
