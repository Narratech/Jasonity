using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    public class Plan: Term
    {
        private Term plan;
        private string typeOfPlan;

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
