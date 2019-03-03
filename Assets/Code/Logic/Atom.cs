using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Atom : Literal
    {
        public Atom(string functor, bool b) : base(functor, b) { }

        public override bool IsAtom()
        {
            return true;
        }
    }
}
