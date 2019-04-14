using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Infra
{
    public interface IMsgListener
    {
        void MsgSent(Message m);
    }
}
