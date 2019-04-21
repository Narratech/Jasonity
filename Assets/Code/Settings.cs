using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public class Settings
    {
        public const int DEFAULT_NUMBER_REASONING_CYCLES = 1;
        internal static readonly object INIT_BELS;
        internal static readonly object INIT_GOALS;
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

        internal bool SameFocus()
        {
            throw new NotImplementedException();
        }

        internal void SetVerbose(int v)
        {
            throw new NotImplementedException();
        }

        internal string GetUserParameter(object iNIT_BELS)
        {
            throw new NotImplementedException();
        }

        internal bool IsSync()
        {
            throw new NotImplementedException();
        }
    }
}
