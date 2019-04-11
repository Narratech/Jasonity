// A Plan and the Unifier that makes it relevant and applicable
using Assets.Code.AsSyntax;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;

namespace BDIManager.Intentions
{
    public class Option
    {

        private Plan plan;
        private Unifier unif;
        private Plan p;
        private Unifier current;

        public Option(Plan p, Unifier u)
        {
            plan = p;
            unif = u;
        }

        public Option(Plan p, Unifier current)
        {
            this.p = p;
            this.current = current;
        }

        public void SetPlan(Plan p)
        {
            plan = p;
        }

        internal Plan GetPlan()
        {
            return plan;
        }

        public void SetUnifier(Unifier u)
        {
            unif = u;
        }

        internal Unifier GetUnifier()
        {
            return unif;
        }
    }
}