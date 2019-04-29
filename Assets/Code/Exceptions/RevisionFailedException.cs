using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Exceptions
{
    class RevisionFailedException : JasonityException 
    {
        public RevisionFailedException() { }

        public RevisionFailedException(string msg) : base(msg) { }

        public RevisionFailedException(string msg, Exception cause) : base(msg, cause) { }
    }
}
