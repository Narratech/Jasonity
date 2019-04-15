// Implements an Intention (which is a stack of IntendedPlans)
using System;
using System.Collections.Generic;
using System.Text;
using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;

namespace BDIManager.Intentions
{
    public class Intention
    {
        public static Intention emptyInt = null;
        private static int idCount = 0;

        private int id;
        private int atomicCount = 0;
        private bool isSuspended = false;
        private string suspendedReason = null;

        private List<IntendedPlan> plans = new List<IntendedPlan>();

        public Intention()
        {
            idCount++;
            id = idCount;
        }

        public int GetID() => id;

        public void Push(IntendedPlan ip)
        {
            plans.Insert(0, ip);
            if (ip.IsAtomic()) atomicCount++;
        }

        public IntendedPlan Peek() => plans[0];

        public IntendedPlan Pop()
        {
            IntendedPlan top = plans[0];
            plans.RemoveAt(0);

            if (IsAtomic() && top.IsAtomic()) atomicCount--;
            return top;
        }

        public bool IsAtomic() => atomicCount > 0;

        public void SetAtomic(int a) => atomicCount = a; // just for testing

        public IEnumerator<IntendedPlan> GetEnumerator() => plans.GetEnumerator();

        public bool IsFinished() => plans.Count == 0;

        public int Size() => plans.Count;

        public void ClearPlans() => plans.Clear();

        public void SetSuspended(bool b)
        {
            isSuspended = b;
            if (!b) suspendedReason = null;
        }

        public bool IsSuspended() => isSuspended;

        public void SetSuspendedReason(string r) => suspendedReason = r;
        public string GetSuspendedReason()
        {
            if (suspendedReason == null) return "";
            else return suspendedReason;
        }

        // Returns the IntendedPlan with TE = g, null if there isn't one
        public IntendedPlan GetIntendedPlan(Trigger g, Unifier u)
        {
            foreach (IntendedPlan ip in plans)
                if (u.Unifies(g, ip.GetTrigger())) return ip;
            return null;
        }

        public IntendedPlan GetBottom() => plans[plans.Count - 1];

        // Returns true if intention has an IP where TE = g, using u to verify equality
        public bool HasTrigger(Trigger g, Unifier u)
        {
            foreach (IntendedPlan ip in plans)
                if (u.Unifies(g, ip.GetTrigger())) return true;
            return false;
        }

        // Remove all IP until the lowest IP with trigger te
        public bool DropDesire(Trigger te, Unifier un)
        {
            bool r = false;
            IntendedPlan ip = GetIntendedPlan(te, un);
            while (ip != null)
            {
                r = true;
                while (Peek() != ip)
                    Pop();
                Pop(); // Remove IP
                ip = GetIntendedPlan(te, un); // Keep removing other occurrences of te
            }
            return r;
        }

        public void Fail(Circumstance c) { }

        public KeyValuePair<Event, int> FindEventForFailure(Trigger tevent, PlanLibrary pl, Circumstance c)
        {
            Trigger failTrigger = new Trigger(TEOperator.del, tevent.GetTEType(), tevent.GetLiteral());
            IEnumerator<IntendedPlan> ii = GetEnumerator();
            int posInStak = Size();
            // synchronized (pl.GetLock())
            while (!pl.HasCandidatePlan(failTrigger) && ii.MoveNext())
            {
                IntendedPlan ip = ii.Current;
                tevent = ip.GetTrigger();
                failTrigger = new Trigger(TEOperator.del, tevent.GetTEType(), tevent.GetLiteral());
                posInStak--;
            }
            if (tevent.IsGoal() && tevent.IsAddition() && pl.HasCandidatePlan(failTrigger))
                return new KeyValuePair<Event, int>(new Event(failTrigger.Clone(), this), posInStak);
            else
                return new KeyValuePair<Event, int>(null, 0);
        }

        // Implements atomic intentions greater than non-atomic intentions
        public int CompareTo(Intention o)
        {
            if (o.atomicCount > atomicCount) return 1;
            if (atomicCount > o.atomicCount) return -1;

            if (o.id > id) return 1;
            if (id > o.id) return -1;
            return 0;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o == this) return true;
            if (o is Intention) return ((Intention)o).id == id;
            return false;
        }

        public int HashCode() => id;

        public Intention Clone()
        {
            Intention i = new Intention();
            i.id = id;
            i.atomicCount = atomicCount;
            i.plans = new List<IntendedPlan>();
            foreach (IntendedPlan ip in plans)
                i.plans.Add((IntendedPlan)ip.Clone());
            return i;
        }

        // Used by fork
        public void CopyTo(Intention i)
        {
            i.atomicCount = atomicCount;
            i.plans = new List<IntendedPlan>(plans);
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("intention " + id + ": \n");
            int i = 0;
            foreach (IntendedPlan ip in plans)
            {
                s.Append("    " + ip + "\n");
                if (i++ > 40)
                {
                    s.Append("... more " + (Size() - 40) + " intended means!\n");
                    break;
                }
            }
            if (IsFinished())
                s.Append("<finished intention>");
            return s.ToString();
        }

        public ITerm GetAsTerm()
        {
            Structure intention = new Structure("intention");
            intention.AddTerm(new NumberTermImpl(GetID()));
            IListTerm lt = new ListTermImpl();
            foreach (IntendedPlan ip in plans)
                lt.Add(ip.GetAsTerm());
            intention.AddTerm(lt);
            return intention;
        }
    }
}
