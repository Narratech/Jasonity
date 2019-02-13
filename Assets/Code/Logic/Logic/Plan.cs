using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Plan : Term
    {
        public Plan(string name) : base(name)
        {
        }

        public override bool IsPlan()
        {
            return true;
        }
    }
}
