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
        
        private string actionFocus;

        public Plan(Objective objective, Belief belief, string name, string aFocus) : base(name)
        {
            this.condition = belief;
            this.planObjective = objective;
            this.actionFocus = aFocus;
        }

        public override bool IsPlan()
        {
            return true;
        }

        public Belief Condition { get => this.condition; }

        public Objective PlanObjective { get => this.planObjective; }

        public string AFocus { get => this.actionFocus; }
    }
}
