using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.ReasoningCycle;

namespace Assets.Code.Logic
{
    public class Plan : Term
    {
        private Belief condition;
        private Objective planObjective;
        private string actionFocus;

        public Plan(Objective objective, Belief belief, string name, string aFocus) : base(name)
        {
            this.condition = belief;
            this.planObjective = objective;
            this.actionFocus = aFocus;
        }

        internal PlanBody getBody()
        {
            throw new NotImplementedException();
        }

        public override bool IsPlan()
        {
            return true;
        }

        public Belief Condition { get => this.condition; }

        internal object GetTrigger()
        {
            throw new NotImplementedException();
        }

        public Objective PlanObjective { get => this.planObjective; }

        public string AFocus { get => this.actionFocus; }

        internal bool IsAtomic()
        {
            throw new NotImplementedException();
        }

        internal Unifier IsRelevant(Trigger trigger)
        {
            throw new NotImplementedException();
        }
    }
}
