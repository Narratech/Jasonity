using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jason.AsSemantics.Entities
{
    [Serializable]
    public class ActionExec
    {
        private Literal action;
        private Intention intention;
        private bool result;
        private Literal failureReason;
        private string failureMsg;

        public ActionExec(Literal ac, Intention i)
        {
            action = ac;
            intention = i;
            result = false;
        }

        //Overrride
        public bool equals(Object ao)
        {
            if (ao == null) return false;
            if (!(ao.GetType().IsInstanceOfType(this/*ActionExec*/))) return false;
            ActionExec a = (ActionExec)ao;
            return action.equals(a.action);
        }

        //Override
        public int hashCode()
        {
            return action.hashCode();
        }

        public Structure getActionTerm()
        {
            if (action.GetType().IsInstanceOfType(Structure))
                return (Structure)action;
            else
                return new Structure(action);
        }

        public Intention getIntention()
        {
            return intention;
        }
        public bool getResult()
        {
            return result;
        }
        public void setResult(bool ok)
        {
            result = ok;
        }

        public void setFailureReason(Literal reason, string msg)
        {
            failureReason = reason;
            failureMsg = msg;
        }
        public string getFailureMsg()
        {
            return failureMsg;
        }
        public Literal getFailureReason()
        {
            return failureReason;
        }

        public string toString()
        {
            return "<" + action + "," + intention + "," + result + ">";
        }

        protected ActionExec clone()
        {
            ActionExec ae = new ActionExec((Pred)action.clone(), intention.clone());
            ae.result = this.result;
            return ae;
        }

        /** get as XML */
        public Element getAsDOM(Document document)
        {
            Element eact = (Element)document.createElement("action");
            eact.setAttribute("term", action.toString());
            eact.setAttribute("result", result + "");
            eact.setAttribute("intention", intention.getId() + "");
            return eact;
        }
    }
}
