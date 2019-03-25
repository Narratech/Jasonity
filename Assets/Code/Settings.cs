using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    class Settings
    {
        public const int DEFAULT_NUMBER_REASONING_CYCLES = 1;

        private int nrcbp = DEFAULT_NUMBER_REASONING_CYCLES;


        public Settings()
        {

        }

        public int Nrcbp()
        {
            return nrcbp;
        }

        public void SetNrcbp(int n)
        {
            nrcbp = n;
        }

        internal int Verbose()
        {
            throw new NotImplementedException();
        }

        internal bool Retrieve()
        {
            throw new NotImplementedException();
        }

        internal bool Requeue()
        {
            throw new NotImplementedException();
        }

        internal bool IsTROon()
        {
            throw new NotImplementedException();
        }
    }
}
