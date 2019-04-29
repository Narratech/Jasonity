namespace Assets.Code.AsSemantics
{
    public class Message
    {
        private string ilForce = null;
        private string sender = null;
        private string receiver = null;
        private object propCont = null;
        private string msgID = null;
        private string inReplyTo = null;

        private static int idCount = 0; // Supposed to be AtomicInteger, working around it with int

        public static string[] knownPerformatives = { "tell", "untell", "achieve", "unachieve", "askOne", "askAll", "tellHow", "untellHow", "askHow" };

        public static string msgIDPrefix = "mid";
        public static string msgIDSyncAskPrefix = "samid";

        public static string kqmlReceivedFunctor = "kqml_received";
        public static string kqmlDefaultPlans = "$jasonityJar/asl/kqmlPlans.asl";

        public Message() { }

        public Message(string ilf, string s, string r, object c)
        {
            idCount++;
            new Message(ilf, s, r, c, msgIDPrefix + (idCount));
        }

        public Message(string ilf, string s, string r, object c, string id)
        {
            SetIlForce(ilf);
            sender = s;
            receiver = r;
            propCont = c;
            msgID = id;
        }

        public Message(Message m)
        {
            ilForce = m.ilForce;
            sender = m.sender;
            receiver = m.receiver;
            propCont = m.propCont;
            msgID = m.msgID;
            inReplyTo = m.inReplyTo;
        }

        public void SetSyncAskMsgID()
        {
            idCount++;
            msgID = msgIDSyncAskPrefix + (idCount);
        }

        public string GetIlForce() => ilForce;

        public void SetIlForce(string ilf)
        {
            if (ilf.Equals("ask-one")) ilf = "askOne";
            if (ilf.Equals("ask-all")) ilf = "askAll";
            if (ilf.Equals("tell-how")) ilf = "tellHow";
            if (ilf.Equals("ask-how")) ilf = "askHow";
            if (ilf.Equals("untell-how")) ilf = "untellHow";
            ilForce = ilf;
        }

        public bool IsAsk() => ilForce.StartsWith("ask");
        public bool IsTell() => ilForce.StartsWith("tell");
        public bool IsUntell() => ilForce.StartsWith("untell");

        public bool IsReplyToSyncAsk() => inReplyTo != null && inReplyTo.StartsWith(msgIDSyncAskPrefix);

        public bool IsKnownPerformative()
        {
            foreach (string s in knownPerformatives)
            {
                if (ilForce.Equals(s)) return true;
            }
            return false;
        }

        public void SetPropCont(object o) => propCont = o;
        public object GetPropCont() => propCont;

        public string GetReceiver() => receiver;
        public void SetSender(string agName) => sender = agName;
        public string GetSender() => sender;
        public void SetReceiver(string agName) => receiver = agName;

        public string GetMsgID() => msgID;
        public void SetMsgID(string id) => msgID = id;

        public string GetInReplyTo() => inReplyTo;
        public void SetInReplyTo(string inReplyTo) => this.inReplyTo = inReplyTo;

        public Message Clone() => new Message(this);

        // Creates a new message object based on a string that follows the format of the ToString of Message
        public static Message ParseMsg(string msg)
        {
            int one, two;
            Message newmsg = new Message();
            if (msg.StartsWith("<"))
            {
                one = msg.IndexOf(",");
                int arrowIndex = msg.IndexOf("->");
                // If there is an arrow before the first comma
                if ((arrowIndex > 0) && (one > arrowIndex))
                {
                    newmsg.msgID = msg.Substring(1, arrowIndex);
                    newmsg.inReplyTo = msg.Substring(arrowIndex + 2, one);
                }
                else newmsg.msgID = msg.Substring(1, one);
                two = msg.IndexOf(",", one + 1);
                newmsg.sender = msg.Substring(one + 1, two);
                one = msg.IndexOf(",", two + 1);
                newmsg.ilForce = msg.Substring(two + 1, one);
                two = msg.IndexOf(",", one + 1);
                newmsg.receiver = msg.Substring(one + 1, two);
                one = msg.IndexOf(">", two + 1);
                string content = msg.Substring(two + 1, one);
                newmsg.propCont = AsSyntax.AsSyntax.ParseTerm(content);
            }
            return newmsg;
        }

        public override string ToString()
        {
            string irt = (inReplyTo == null ? "" : "->" + inReplyTo);
            return "<" + msgID + irt + "," + sender + "," + ilForce + "," + receiver + "," + propCont + ">";
        }

        public string GetMessageId()
        {
            return msgID;
        }
    }
}
