using Assets.Code.Logic;
using System;
using System.Collections.Generic;

namespace BDIManager.Intentions
{
    class PlanLibrary
    {
        private bool hasMetaEventPlans = false;

        public static Trigger TE_JAG_SLEEPING = new Trigger();
        public static Trigger TE_JAG_AWAKING = new Trigger();

        // A dictionary from TE to a list of relevant plans
        private Dictionary<PredicateIndicator, List<Plan>> relPlans = new Dictionary<PredicateIndicator, List<Plan>>();

        // Plans that have var as TE
        private List<Plan> varPlans = new List<Plan>();

        public bool HasMetaEventPlans()
        {
            return hasMetaEventPlans;
        }

        public List<Plan> GetCandidatePlans(Trigger trigger)
        {
            List<Plan> l = null;
            if (trigger.GetLiteral().IsVar() || trigger.GetNS().IsVar())
            {
                foreach (var p in this) // ???
                {
                    if (p.GetTrigger().GetType() == trigger.GetType())
                    {
                        if (l == null)
                        {
                            l = new List<Plan>();
                        }
                        l.Add(p);
                    }
                }
            }
            else
            {
                l = relPlans[trigger.GetPredicateIndicator()];
                if ((l == null || l.Count == 0) && !(varPlans.Count == 0) && trigger != TE_JAG_SLEEPING && trigger != TE_JAG_AWAKING)
                {
                    foreach (Plan p in varPlans)
                    {
                        if (p.GetTrigger().GetType() == typeof(Trigger))
                        {
                            if (l == null)
                            {
                                l = new List<Plan>();
                            }
                        }
                    }
                }
            }
            // If no relevant plan, return null instead of empty list
            return l;
        }

        public bool HasCandidatePlan(Trigger t)
        {
            return t == null ? false : GetCandidatePlans(t) != null;
        }
    }
}
