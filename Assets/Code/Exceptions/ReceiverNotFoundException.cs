using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Exceptions
{
    class ReceiverNotFoundException : Exception
    {
        public ReceiverNotFoundException() { }

        public ReceiverNotFoundException(string msg) : base(msg) { }

        public ReceiverNotFoundException(string msg, Exception cause) : base(msg, cause) { }
    }
}
