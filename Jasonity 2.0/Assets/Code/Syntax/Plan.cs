using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Plan
    {
        private Trigger trigger;
        private string context;
        private PlanBody planBody;

        public Plan(Trigger trigger, string context, PlanBody planBody)
        {
            this.trigger = trigger;
            this.context = context;
            this.planBody = planBody;
        }

        public Trigger Trigger { get => trigger; }

        public string Context { get => context; }

        public PlanBody PlanBody { get => planBody; }
    }
}
