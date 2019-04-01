using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic.AsSyntax
{
    public class LiteralImpl : Pred
    {
        private Literal t;
        private Unifier u;

        public LiteralImpl(string s)
        {
        }

        public LiteralImpl(Literal t)
        {
            this.t = t;
        }

        public LiteralImpl(Literal t, Unifier u) : this(t)
        {
            this.u = u;
        }
    }
}
