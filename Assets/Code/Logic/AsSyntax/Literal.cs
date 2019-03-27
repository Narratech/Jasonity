using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Literal : DefaultTerm
    {
        public override bool IsLiteral()
        {
            return true;
        }
    }
}
