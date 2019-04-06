using Assets.Code.Logic;
using Assets.Code.Logic.AsSyntax;
using BDIManager.Beliefs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDIManager.Intentions
{
    class PlanLibrary
    {
        // A dictionary from TE to a list of relevant plans
        private Dictionary<PredicateIndicator, List<Plan>> relPlans = new Dictionary<PredicateIndicator, List<Plan>>();

        // All defined plans
        private List<Plan> plans = new List<Plan>();

        // Plans that have var as TE
        private List<Plan> varPlans = new List<Plan>();

        // A dictionary from labels to plans
        private Dictionary<string, Plan> planLabels = new Dictionary<string, Plan>();

        private bool hasMetaEventPlans = false;

        public static Trigger TE_JAG_SLEEPING = new Trigger();
        public static Trigger TE_JAG_AWAKING = new Trigger();

        // TODO: private AtomicInteger lastPlanLabel = new AtomicInteger(0);

        private bool hasUserKqmlReceived = false;

        private object lockPL = new object();

        public object GetLock() => lockPL;

        // Adds a new plan written as a string. Source is usually "self" or the agent who sent this plan.
        // New plans are added at the end of the library.
        public Plan Add(StringTerm stPlan, Term tSource) => Add(stPlan, tSource, false);

        // Same as previous Add. If "before" is true, add at the beginning of the library.
        public Plan Add(StringTerm stPlan, Term tSource, bool before)
        {
            string sPlan = stPlan.GetString();
            // Remove quotes
            StringBuilder sTemp = new StringBuilder();
            for (int c = 0; c < sPlan.Length; c++)
            {
                if (sPlan[c] != '\\')
                {
                    sTemp.Append(sPlan[c]);
                }
            }
            sPlan = sTemp.ToString();
            Plan p = ASSyntax.ParsePlan(sPlan); // ???
            return Add(p, tSource, before);
        }

        // Adds a new Plan to the library. If "before" is true, add at the beginning of the library.
        public Plan Add(Plan p, Term tSource, bool before)
        {
            // synchronized(lockPL)
            int i = plans.IndexOf(p);
            if (i < 0)
            {
                // adds label if necessary
                if (p.GetLabel() == null)
                {
                    p.SetLabel(GetUniqueLabel());
                }
                p.GetLabel().AddSource(tSource);    // Check after implementing Plan
                Add(p, before);
            }
            else
            {
                p = plans[i];
                p.GetLabel().AddSource(tSource);
            }
            return p;
        }

        public void Add(Plan p)
        {
            Add(p, false);
        }

        private string kqmlReceivedFunctor = Config.Get().GetKqmlFunctor(); // ???

        public void Add(Plan p, bool before)
        {
            // synchronized (lockPL)
            if (p.GetLabel() != null && planLabels.ContainsKey(GetStringForLabel(p.GetLabel())))
            {
                // If the new plan is equal, add a source
                Plan planInPL = Get(p.GetLabel());
                if (p.Equals(planInPL))
                {
                    planInPL.GetLabel().AddSource(p.GetLabel().GetSources().Get(0));
                    return;
                }
                else
                {
                    throw new Exception("There already is a plan with this label.");
                }
            }

            // adds label if necessary
            if (p.GetLabel() == null)
            {
                p.SetLabel(GetUniqueLabel());
            }

            // adds self source
            if (!p.GetLabel().HasSource())
            {
                p.GetLabel().AddAnnot(DefaultBeliefBase.TSelf);
            }

            if (p.GetTrigger().GetLiteral().GetFunctor().Equals(kqmlReceivedFunctor))
            {
                if (!(p.GetSrcInfo() != null && "kmqlPlans.asl".Equals(p.GetSrcInfo().GetSrcFile())))
                {
                    hasUserKqmlReceived = true;
                }
            }

            p.SetAsPlanTerm(false); // not a term anymore

            planLabels.Add(GetStringForLabel(p.GetLabel()), p);

            Trigger pte = p.GetTrigger();
            if (pte.GetLiteral().IsVar() || pte.GetLiteral().GetNS().IsVar())
            {
                if (before)
                {
                    varPlans.Add(0, p);
                }
                else
                {
                    varPlans.Add(p);
                }
                // Add plan p in all entries
                foreach (List<Plan> lp in relPlans.Values)
                {
                    if (!(lp.Count == 0) && lp[0].GetTrigger().SameType(pte))
                    {
                        if (before)
                        {
                            lp.Add(0, p);
                        }
                        else
                        {
                            lp.Add(p);
                        }
                    }
                }
            }
            else
            {
                List<Plan> codesList = relPlans[pte.GetPredicateIndicator()];
                if (codesList == null)
                {
                    codesList = new List<Plan>();
                    // Copy plans from var plans
                    foreach (Plan vp in varPlans)
                    {
                        if (vp.GetTrigger().SameType(pte))
                        {
                            codesList.Add(vp);
                        }
                    }
                    relPlans.Add(pte.GetPredicateIndicator(), codesList);
                }
                if (before)
                {
                    codesList.Add(0, p);
                }
                else
                {
                    codesList.Add(p);
                }
            }

            if (pte.GetOperator() == TEOperator.GoalState)
            {
                hasMetaEventPlans = true;
            }

            if (before)
            {
                plans.Add(0, p);
            }
            else
            {
                plans.Add(p);
            }
        }

        public void AddAll(PlanLibrary pl)
        {
            throw new NotImplementedException();
        }

        private Plan Get(Pred pred)
        {
            throw new NotImplementedException();
        }

        private string GetStringForLabel(Pred pred)
        {
            throw new NotImplementedException();
        }

        private Pred GetUniqueLabel()
        {
            throw new NotImplementedException();
        }

        public bool HasMetaEventPlans() => hasMetaEventPlans;

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
