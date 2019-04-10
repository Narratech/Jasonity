// Implements an Intention (which is a stack of IntendedPlans)
using System;
using System.Collections.Generic;
using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;

namespace BDIManager.Intentions
{
    class Intention
    {
        public static Intention emptyInt = null;
        private static int id = 0;
        private int count = 0;

        private List<IntendedPlan> plans = new List<IntendedPlan>();
        private string suspendedReason = null;

        public Intention()
        {
            id++;
        }

        public int GetID()
        {
            return id;
        }

        public void Push(IntendedPlan plan)
        {
            plans.Add(plan);
            count++;
        }

        public IntendedPlan Pop()
        {
            // Plan top = plans.Remove(plans.FindLastIndex());
            IntendedPlan top = null;
            if (IsAtomic() && top.IsAtomic())
            {
                count--;
            }
            return top;
        }

        public bool IsAtomic()
        {
            return count > 0;
        }

        public bool IsFinished()
        {
            return plans.Count == 0;
        }

        public int Size()
        {
            return plans.Count;
        }

        public void clearIM()
        {
            plans.Clear();
        }

        public IntendedPlan GetBottom()
        {
            // ??? return plans.GetLast(); 
            return null;
        }

        public bool HasTrigger(Trigger t, Unifier unifier)
        {
            foreach (var p in plans)
            {
                if (unifier.Unifies(t, p.GetTrigger()))
                {
                    return true;
                }
            }
            return false;
        }

        // Returns the head of the list, null if empty
        public IntendedPlan Peek()
        {
            if (plans.Count == 0)
            {
                return null;
            }
            return plans[0];
        }

        public IEnumerable<IntendedPlan> GetIntendedPlan()
        {
            return plans;
        }

        public void SetSuspendedReason(string r)
        {
            suspendedReason = r;
        }

        // This function is empty in the original code. Should it do something?
        internal void Fail(Circumstance circumstance)
        {
            
        }

        internal Event FindEventForFailure(Trigger trigger, PlanLibrary planLibrary, Circumstance c)
        {
            Trigger failTrigger = new Trigger(TEOperator.del, trigger.GetTEType(), trigger.GetLiteral());
            int posInStack = Size();
            // Synchronized???
            throw new NotImplementedException();
        }

        internal bool DropDesire(Trigger te, Unifier u)
        {
            bool r = false;
            IntendedPlan p = GetIntendedPlan(te, u);
            while (p != null)
            {
                r = true;
                // Remove until p-1
                while (Peek() != p)
                {
                    Pop();
                }
                Pop(); // Remove plan
                p = GetIntendedPlan(te, u);
            }
            return r;
        }

        private IntendedPlan GetIntendedPlan(Trigger te, Unifier u)
        {
            foreach (var p in plans)
            {
                if (u.Unifies(te, p.GetTrigger()))
                {
                    return p;
                }
            }
            return null;
        }
    }
}
