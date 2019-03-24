// A Plan and the Unifier that makes it relevant and applicable
using Assets.Code.Logic;

namespace BDIManager.Intentions
{
    public class Option
    {

        private Plan plan;
        private Unifier unif;

        Option(Plan p, Unifier u)
        {
            plan = p;
            unif = u;
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

        internal Unifier getUnifier()
        {
            return unif;
        }
    }
}