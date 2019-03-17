// Implements an Intention (which is a stack of IntendedPlans)
using System.Collections.Generic;

namespace BDIManager.Intentions
{
    public class Intention
    {
        public static Intention emptyInt = null;
        private static int id = 0;
        private int count = 0;

        private List<IntendedPlan> plans = new List<IntendedPlan>();

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
            plans.Push(plan);
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
    }
}
