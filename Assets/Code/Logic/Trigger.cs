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
    }
}
