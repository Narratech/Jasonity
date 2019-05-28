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
        string typeOfDesire; //Test or not

        public Desire(Term d, string s)
        {
            this.desire = d;
            this.typeOfDesire = s;
        }
    }
}
