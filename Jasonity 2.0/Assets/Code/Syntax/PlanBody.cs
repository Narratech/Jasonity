using System.Collections.Generic;

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
