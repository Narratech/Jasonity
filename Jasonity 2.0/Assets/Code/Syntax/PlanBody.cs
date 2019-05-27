using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    public class PlanBody:Term
    {
        List<Action> actions;

        public PlanBody(List<Action> actions)
        {
            this.actions = actions;
        }

        public List<Action> Actions { get => actions; }

        public override bool IsPlanBody()
        {
            return true;
        }
    }
}
