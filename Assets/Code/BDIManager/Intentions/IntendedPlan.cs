// Interface for plans
// Allows the user to modify and check an agent's plans
using System;
using Assets.Code.Logic;

namespace BDIManager.Intentions {
    public class IntendedPlan
    {
        protected PlanBody planBody;
        protected Plan plan;
        private Trigger trigger;

        IntendedPlan(Option opt, Trigger te)
        {
            plan = opt.GetPlan();
            planBody = plan.getBody();
        }

        public Term RemoveCurrentStep()
        {
            if (IsFinished())
            {
                return null;
            }
            else
            {
                Term r = planBody.GetBodyTerm();
                planBody = planBody.GetBodyNext();
                return r;
            }
        }

        public PlanBody GetCurrentStep()
        {
            return planBody;
        }

        public PlanBody InsertAsNextStep(PlanBody pb)
        {
            planBody = new PlanBodyImpl(planBody.getBodyType(), planBody.GetBodyTerm());
            planBody.SetBodyNext(pb);
            return planBody;
        }

        public Plan GetPlan()
        {
            return plan;
        }

        Trigger GetTrigger()
        {
            return trigger;
        }

        void SetTrigger(Trigger tr)
        {
            trigger = tr;
        }

        internal bool IsAtomic()
        {
            return plan != null && plan.IsAtomic();
        }

        private bool IsFinished()
        {
            return planBody == null || planBody.IsEmptyBody();
        }

        public bool IsGoalAdd()
        {
            return trigger.IsAddition() && trigger.IsGoal();
        }
    }
}