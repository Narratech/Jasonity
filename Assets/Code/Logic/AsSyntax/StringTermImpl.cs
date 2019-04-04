using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic.AsSyntax
{
    public class StringTermImpl : DefaultTerm, StringTerm
    {
        private string v;

        public StringTermImpl(string v)
        {
            this.v = v;
        }
    }
}
