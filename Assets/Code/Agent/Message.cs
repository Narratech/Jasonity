using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Agent
{
    class Message
    {
        private object propCont = null;
        private string inReplyTo = null;

        public string[] knownPerformatives = { "tell", "untell", "achieve", "unachieve", "askOne", "askAll", "tellHow", "untellHow", "askHow" };
        private string ilForce = null;
        private string sender = null;

        void SetPropCont(object o)
        {
            propCont = o;
        }

        internal object GetPropCont()
        {
            return propCont;
        }

        internal string GetInReplyTo()
        {
            return inReplyTo;
        }

        void SetInReplyTo(string inReplyTo)
        {
            this.inReplyTo = inReplyTo;
        }

        internal bool IsUntell()
        {
            return ilForce.StartsWith("untell");
        }

        internal string GetSender()
        {
            return sender;
        }

        void SetSender(string agName)
        {
            sender = agName;
        }
    }
}
