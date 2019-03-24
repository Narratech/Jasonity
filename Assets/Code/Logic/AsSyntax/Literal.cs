using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Literal : DefaultTerm
    {
        private bool imNotFalse;
        private DefaultNameSpace defaultNS;

        public Literal(string functor, bool b)
        {
            this.imNotFalse = b;
            this.defaultNS = new DefaultNameSpace(functor);
        }

        public bool IsItFalse { get => this.imNotFalse; }

        public DefaultNameSpace DefaultNS { get => this.defaultNS; }

        public override bool IsLiteral()
        {
            return true;
        }

        public class DefaultNameSpace
        {
            private string functor;

            public DefaultNameSpace(string functor)
            {
                this.functor = functor;
            }

            public string Functor { get => this.functor; }
        }
    }
}
