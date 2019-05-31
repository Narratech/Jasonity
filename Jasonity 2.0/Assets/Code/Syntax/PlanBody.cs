using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class PlanBody
    {
        List<Action> actions;

        public PlanBody(List<Action> actions)
        {
            this.actions = actions;
        }

        public List<Action> Actions { get => actions; }

    }
}
