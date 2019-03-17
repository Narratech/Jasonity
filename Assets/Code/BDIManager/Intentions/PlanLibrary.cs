using Assets.Code.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDIManager.Intentions
{
    class PlanLibrary
    {

        public static Trigger TE_JAG_SLEEPING = new Trigger();
        public static Trigger TE_JAG_AWAKING = new Trigger();

        public bool hasMetaEventPlans()
        {
            throw new NotImplementedException();
        }

        public List<Plan> GetCandidatePlans(Trigger trigger)
        {
            throw new NotImplementedException();
        }
    }
}
