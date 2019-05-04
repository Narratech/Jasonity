using Assets.Code.Util;
using BDIManager.Beliefs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Assets.Code.AsSyntax
{
    public class PlanLibrary : IEnumerable<Plan>
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

        private int lastPlanLabel = 0;

        private bool hasUserKqmlReceived = false;

        private object lockPL = new object();

        public object GetLock() => lockPL;

        // Adds a new plan written as a string. Source is usually "self" or the agent who sent this plan.
        // New plans are added at the end of the library.
        public Plan Add(IStringTerm stPlan, ITerm tSource) => Add(stPlan, tSource, false);

        // Same as previous Add. If "before" is true, add at the beginning of the library.
         public Plan Add(IStringTerm stPlan, ITerm tSource, bool before)
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
            Plan p = AsSyntax.ParsePlan(sPlan); // ???
            return Add(p, tSource, before);
        }

        // Adds a new Plan to the library. If "before" is true, add at the beginning of the library.
        public Plan Add(Plan p, ITerm tSource, bool before)
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

        private string kqmlReceivedFunctor = Config.Get().GetKqmlFunctor(); 

        public void Add(Plan p, bool before) //Revisarse el nuevo jason
        {
            // synchronized (lockPL)
            if (p.GetLabel() != null && planLabels.ContainsKey(GetStringForLabel(p.GetLabel())))
            {
                // If the new plan is equal, add a source
                Plan planInPL = Get(p.GetLabel());
                if (p.Equals(planInPL))
                {
                    planInPL.GetLabel().AddSource(p.GetLabel().GetSources()[0]);
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
                p.GetLabel().AddAnnot(BeliefBase.TSelf);
            }

            if (p.GetTrigger().GetLiteral().GetFunctor().Equals(kqmlReceivedFunctor))
            {
                if (!(p.GetSrcInfo() != null && "kmqlPlans.asl".Equals(p.GetSrcInfo()/*.GetSrcFile()*/)))
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
                    varPlans.Insert(0, p);
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
                            lp.Insert(0, p);
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
                    codesList.Insert(0, p);
                }
                else
                {
                    codesList.Add(p);
                }
            }

            if (pte.GetOperator() == TEOperator.desireState)
            {
                hasMetaEventPlans = true;
            }

            if (before)
            {
                plans.Insert(0, p);
            }
            else
            {
                plans.Add(p);
            }
        }

        public void AddAll(PlanLibrary pl)
        {
            // Synchronized (lockPL)
            foreach (Plan p in pl)
            {
                Add(p, false);
            }
        }

        public void AddAll(List<Plan> plans)
        {
            // Synchronized (lockPL)
            foreach (Plan p in plans)
            {
                Add(p, false);
            }
        }

        private string GetStringForLabel(Literal p)
        {
            // Use functor + terms
            StringBuilder l = new StringBuilder();
            if (p.GetNS() != Literal.DefaultNS)
            {
                l.Append(p.GetNS() + "::");
            }
            l.Append(p.GetFunctor());
            if (p.HasTerm())
            {
                foreach (ITerm t in p.GetTerms())
                {
                    l.Append(t.ToString());
                }
            }
            return l.ToString();
        }

        public bool HasMetaEventPlans() => hasMetaEventPlans;

        public bool HasUserKqmlReceivedPlans() => hasUserKqmlReceived;

        private Pred GetUniqueLabel()
        {
            string l;
            do
            {
                lastPlanLabel++;
                l = "l__" + (lastPlanLabel);
            } while (planLabels.ContainsKey(l));
            return new Pred(l);
        }

        // Returns a plan for a label
        public Plan Get(string label) => Get(new Atom(label));

        private Plan Get(Literal label) => planLabels[GetStringForLabel(label)];

        public int Size() => plans.Count;

        public List<Plan> GetPlans() => plans;

        // Removes all plans
        public void Clear()
        {
            planLabels.Clear();
            plans.Clear();
            varPlans.Clear();
            relPlans.Clear();
        }

        // Removes a plan represented by the label "pLabel".
        // If a plan has many sources, only the plan's source is removed
        public bool Remove(Literal pLabel, ITerm source)
        {
            // Finds the plan
            Plan p = Get(pLabel);
            if (p != null)
            {
                bool hasSource = p.GetLabel().DelSource(source);
                
                if (hasSource && !p.GetLabel().HasSource())
                {
                    Remove(pLabel);
                }
                return true;
            }
            return false;
        }

        public Plan Remove(Literal pLabel)
        {
            // Synchronized (lockPL)
            Plan p = planLabels[GetStringForLabel(pLabel)];
            planLabels.Remove(GetStringForLabel(pLabel));

            // Removes it from plans list
            plans.Remove(p);

            if (p.GetTrigger().GetLiteral().IsVar())
            {
                varPlans.Remove(p);
                // Removes p from all entries and cleans empty entries

                IEnumerator<PredicateIndicator> ipi = relPlans.Keys.GetEnumerator();
                while (ipi.MoveNext()) {
                    PredicateIndicator pi = ipi.Current;
                    List<Plan> lp = relPlans[pi];
                    lp.Remove(p);
                    if (lp.Count == 0) {
                        ipi.Dispose();
                    }
                }
                 
            }
            else
            {
                List<Plan> codesList = relPlans[p.GetTrigger().GetPredicateIndicator()];
                codesList.Remove(p);
                if (codesList.Count == 0)
                {
                    // No more plans for this TE
                    relPlans.Remove(p.GetTrigger().GetPredicateIndicator());
                }
            }
            return p;
        }

        public bool IsRelevant(Trigger te) => HasCandidatePlan(te);

        public bool HasCandidatePlan(Trigger t)
        {
            return t == null ? false : GetCandidatePlans(t) != null;
        }

        public List<Plan> GetCandidatePlans(Trigger trigger)
        {
            // Synchronized (lockPL)
            List<Plan> l = null;
            if (trigger.GetLiteral().IsVar() || trigger.GetNS().IsVar())
            {
                foreach (var p in this)
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

        public static Trigger TE_JAG_SLEEPING = new Trigger(TEOperator.add, TEType.achieve, new Atom("jag_sleeping"));
        public static Trigger TE_JAG_AWAKING = new Trigger(TEOperator.add, TEType.achieve, new Atom("jag_awaking"));
        
        public PlanLibrary Clone()
        {
            PlanLibrary pl = new PlanLibrary();
            try
            {
                // Synchronized (lockPL)
                foreach (Plan p in this) 
                {
                    pl.Add((Plan)p.Clone(), false);
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return pl;
        }

        public override string ToString() => plans.ToString();

        public string GetAsTxt(bool includeKQMLPlans)
        {
            Dictionary<string, StringBuilder> splans = new Dictionary<string, StringBuilder>();
            StringBuilder r;
            foreach (Plan p in plans)
            {
                r = splans[p.GetSource()];
                if (r == null)
                {
                    r = new StringBuilder();
                    if (p.GetSource().Length == 0)
                    {
                        r.Append("\n\n// plans without file\n\n");
                    }
                    else
                    {
                        r.Append("\n\n// plans from " + p.GetSource() + "\n\n");
                    }
                    splans.Add(p.GetSource(), r);
                }
                r.Append(p.ToString() + "\n");
            }

            r = new StringBuilder();
            StringBuilder end = new StringBuilder("\n");
            foreach (string f in splans.Keys)
            {
                if (f.Contains("kqmlPlans"))
                {
                    if (includeKQMLPlans)
                    {
                        end.Append(splans[f]);
                    }
                    else
                    {
                        continue;
                    }
                }
                if (f.Length == 0)
                {
                    end.Append(splans[f]);
                }
                else
                {
                    r.Append(splans[f]);
                }
            }
            return r.ToString() + end.ToString();
        }

        public IEnumerator<Plan> GetEnumerator()
        {
            return plans.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
