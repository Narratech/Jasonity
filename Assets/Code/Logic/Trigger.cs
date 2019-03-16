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
        internal Literal GetLiteral()
        {
            throw new NotImplementedException();
        }

        internal TEType GetType()
        {

        }

        internal bool IsAddition()
        {
            throw new NotImplementedException();
        }

        internal bool IsGoal()
        {
            throw new NotImplementedException();
        }
    }
}
