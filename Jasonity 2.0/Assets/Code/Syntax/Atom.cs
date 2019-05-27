using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Atom: Literal
    {
        public Atom(string functor, bool believes): 
            base(functor, believes) { }

        public override bool IsAtom()
        {
            return true;
        }
    }
}
