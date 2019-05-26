using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    public class Plan:Term
    {
        private Trigger trigger;
        private Dictionary<Literal, string> context;
        private PlanBody planBody;

        public Plan(Trigger trigger, Dictionary<Literal, string> data)
        {
            this.trigger = trigger;
        }

        public Trigger Trigger { get => trigger; }
    }
}
