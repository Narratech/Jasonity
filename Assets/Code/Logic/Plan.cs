using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Plan : Term
    {
        /*Condition of the plan*/
        private Belief condition;
        /*Objective of the plan*/
        private Objective planObjective;
        /*Focus of the action of the plan*/
        private string actionFocus;

        /*Constructor
            objective: Objective
            belief: Belief
            name: string
            aFocus: focus
        */
        public Plan(Objective objective, Belief belief, string action, string aFocus) : base(action)
        {
            this.condition = belief;
            this.planObjective = objective;
            this.actionFocus = aFocus;
        }

        public override bool IsPlan()
        {
            return true;
        }

        /*Get for the condition of the plan*/
        public Belief Condition { get => this.condition; }
        /*Get for the objective of the plan*/
        public Objective PlanObjective { get => this.planObjective; }
        /*Get for the focus of the action of the plan*/
        public string AFocus { get => this.actionFocus; }
    }
}
