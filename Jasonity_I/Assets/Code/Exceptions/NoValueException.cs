using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Exceptions
{
    class NoValueException : JasonityException
    {
        public NoValueException() { }

        public NoValueException(string msg) : base(msg) { }

        public NoValueException(string msg, Exception cause) : base(msg, cause) { }
    }
}
