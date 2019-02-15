// Implements an Intention (which is a stack of IntentionMeans)
using Logica.ASSemantic;
using System.Collections.Generic;

namespace BDIManager.Intentions {
    public class Intention
    {
        public static Intention emptyInt = null;
        private static int id = 0;
        private int count = 0;

        private List<IntendedMeans> intendedMeans = new List<IntendedMeans>();

        public Intention()
        {
            id++;
        }

        public int GetID()
        {
            return id;
        }

        public void Push(IntendedMeans im)
        {
            intendedMeans.Add(im);
            count++;
        }

        public IntendedMeans Pop()
        {
            // IntendedMeans top = intendedMeans.Remove(intendedMeans.???);
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
            // ???
            return intendedMeans.Count == 0;
        }

        public int Size()
        {
            // ???
            return intendedMeans.Count;
        }

        public void clearIM()
        {
            intendedMeans.Clear();
        }

        public IntendedMeans GetBottom()
        {
            // ??? return intendedMeans.LastIndexOf();
            return null;
        }


    }
}
