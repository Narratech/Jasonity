using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Plan: Term
    {
        private Term plan;
        private string typeOfPlan; //A Belief or a Desire(test or not)

        public Plan(Term plan, string typeOfPlan)
        {
            this.plan = plan;
            this.typeOfPlan = typeOfPlan;
        }

        public Term Plam { get => this.plan; }

        public string TypeOfPlan { get => this.typeOfPlan; }

        public override bool IsPlan()
        {
            return true;
        }
    }
}
