// Interface for plans
// Allows the user to modify and check an agent's plans
// Previously IntendedMeans, was renamed
using System;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;

namespace BDIManager.Intentions
{
    public class IntendedPlan
    {
        private Unifier unif = null;
        protected IPlanBody planBody;
        protected Plan plan;
        private Trigger trigger;

        private Unifier renamedVars = null;

        public IntendedPlan(Option opt, Trigger te)
        {
            plan = opt.GetPlan();
            planBody = plan.GetBody();
            unif = opt.GetUnifier();

            if (te == null)
                trigger = plan.GetTrigger().Capply(unif);
            else
                trigger = te.Capply(unif);
        }

        // Used by clone
        private IntendedPlan() { }

        public ITerm RemoveCurrentStep()
        {
            if (IsFinished()) return null;
            else
            {
                ITerm r = planBody.GetBodyTerm();
                planBody = planBody.GetBodyNext();
                return r;
            }
        }

        public IPlanBody GetCurrentStep() => planBody;

        // Used by if/for/loop internal actions
        public IPlanBody InsertAsNextStep(IPlanBody pb)
        {
            planBody = new PlanBodyImpl(planBody.GetBodyType(), planBody.GetBodyTerm());
            planBody.SetBodyNext(pb);
            return planBody;
        }

        public Plan GetPlan() => plan;

        public void SetUnif(Unifier unif) => this.unif = unif;

        public Unifier GetUnif() => unif;

        public Trigger GetTrigger() => trigger;

        public void SetTrigger(Trigger tr) => trigger = tr;

        public bool IsAtomic() => plan != null && plan.IsAtomic();

        public bool IsFinished() => planBody == null || planBody.IsEmptyBody();

        public bool IsGoalAdd() => trigger.IsAddition() && trigger.IsGoal();

        public object Clone()
        {
            IntendedPlan c = new IntendedPlan();
            c.unif = unif.Clone();
            if (planBody != null)
                c.planBody = planBody.ClonePB();
            c.trigger = trigger.Clone();
            c.plan = plan;
            return c;
        }

        public override string ToString() => trigger + " <- " + (planBody == null ? "." : "... " + planBody) + " / " + unif;

        public ITerm GetAsTerm()
        {
            if (planBody is PlanBodyImpl || planBody == null)
            {
                IPlanBody bd;
                if (planBody == null)
                    bd = new PlanBodyImpl();
                else
                    bd = (IPlanBody)((PlanBodyImpl)planBody.Clone()).MakeVarsAnnon();
                bd.SetAsBodyTerm(true);
                Trigger te = GetTrigger().Clone();
                te.SetAsTriggerTerm(true);
                return AsSyntax.CreateStructure("im", AsSyntax.CreateString(plan.GetLabel()), te, bd, unif.GetAsTerm());
            }
            else
                return AsSyntax.CreateAtom("noimplementedforclass" + planBody.GetType().Name);
        }

        public Unifier GetRenamedVars()
        {
            return renamedVars;
        }

        public void SetRenamedVars(Unifier renamedVars)
        {
            this.renamedVars = renamedVars;
        }

        public void Pop()
        {
            throw new NotImplementedException();
        }

        internal IntendedPlan Peek()
        {
            throw new NotImplementedException();
        }
    }
}