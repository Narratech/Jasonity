// Interface for plans
// Allows the user to modify and check an agent's plans
using System;

namespace BDIManager.Intentions {
    public class Plan
    {
        internal static int size;

        public Plan()
        {

        }

        public void Push(Plan plan)
        {

        }

        internal bool IsAtomic()
        {
            throw new NotImplementedException();
        }
    }
}