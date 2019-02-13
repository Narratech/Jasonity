using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Subject : Term
    {
        public Subject(string name) : base(name)
        {
        }

        public override bool IsSubject()
        {
            return true;
        }
    }
}
