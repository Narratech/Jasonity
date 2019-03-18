using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public enum TEType { belief, achieve, test };

    class Trigger
    {
        public Literal GetLiteral()
        {
            throw new NotImplementedException();
        }

        public void GetType()
        {
            throw new NotImplementedException();
        }

        public bool IsAddition()
        {
            throw new NotImplementedException();
        }

        public bool IsGoal()
        {
            throw new NotImplementedException();
        }

        public bool IsMetaEvent()
        {
            throw new NotImplementedException();
        }
    }
}
