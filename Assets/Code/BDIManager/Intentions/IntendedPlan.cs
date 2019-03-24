// Interface for plans
// Allows the user to modify and check an agent's plans
// Previously IntendedMeans, was renamed
using System;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;

namespace BDIManager.Intentions {
    class IntendedPlan
    {
        protected PlanBody planBody;
        protected Plan plan;
        private Trigger trigger;

        public IntendedPlan(Option opt, Trigger te)
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
            planBody = new PlanBodyImpl(planBody.GetBodyType(), planBody.GetBodyTerm());
            planBody.SetBodyNext(pb);
            return planBody;
        }

        public Plan GetPlan()
        {
            return plan;
        }

        public Trigger GetTrigger()
        {
            return trigger;
        }

        public void SetTrigger(Trigger tr)
        {
            trigger = tr;
        }

        public bool IsAtomic()
        {
            return plan != null && plan.IsAtomic();
        }

        public bool IsFinished()
        {
            return planBody == null || planBody.IsEmptyBody();
        }

        public bool IsGoalAdd()
        {
            return trigger.IsAddition() && trigger.IsGoal();
        }

        public Unifier GetUnify()
        {
            throw new NotImplementedException();
        }

        public Unifier GetUnifier()
        {
            throw new NotImplementedException();
        }

        internal object GetRenamedVars()
        {
            throw new NotImplementedException();
        }

        internal Unifier GetUnif()
        {
            throw new NotImplementedException();
        }
    }
}