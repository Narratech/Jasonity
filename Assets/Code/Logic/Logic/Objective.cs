using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Objective : Term
    {
        private ObjectOfTheObjective @object;

        public Objective(string name, string @object) : base(name)
        {
            this.@object = new ObjectOfTheObjective(@object);
        }

        public override bool IsObjective()
        {
            return true;
        }
    }
}
