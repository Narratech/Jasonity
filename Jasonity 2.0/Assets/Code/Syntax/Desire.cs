using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Desire
    {
        Term desire;
        string typeOfTest;

        public Desire(Term d, string s)
        {
            this.desire = d;
            this.typeOfTest = s;
        }
    }
}
