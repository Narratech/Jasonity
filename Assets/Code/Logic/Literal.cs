using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Literal: Term
    {
        private bool isItFalse;

        public Literal(string functor, bool b) : base(functor)
        {
            this.isItFalse = b;
        }

        public bool IsItFalse { get => this.isItFalse; }
    }
}
