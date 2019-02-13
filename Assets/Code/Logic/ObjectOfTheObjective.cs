using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class ObjectOfTheObjective : Term
    {
        public ObjectOfTheObjective(string name) : base(name)
        {
        }

        public override bool IsObjectOfTheObjective()
        {
            return true;
        }
    }


}
