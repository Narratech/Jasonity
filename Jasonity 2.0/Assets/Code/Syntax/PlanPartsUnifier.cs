using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    public class PlanPartsUnifier:Term
    {
        private Trigger trigger;
        private Dictionary<Term, string> context;
        private PlanBody planBody;

        public PlanPartsUnifier(Trigger trigger, Dictionary<Term, string> context, PlanBody planBody)
        {
            this.trigger = trigger;
            this.context = context;
            this.planBody = planBody;
        }

        public Trigger Trigger { get => trigger; }

        public Dictionary<Term, string> Context { get => context; }

        public PlanBody PlanBody { get => planBody; }
    }
}
