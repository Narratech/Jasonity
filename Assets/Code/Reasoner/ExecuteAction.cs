// Executes BDI
// Will use other BDIManager classes to handle BB, plans, desires, events, etc.
using System;
using Assets.Code.AsSyntax;
using BDIManager.Intentions;

namespace BDIMaAssets.Code.ReasoningCycle
{
    [Serializable]
    public class ExecuteAction
    {
        private Literal action;
        private Intention intention;
        private bool result;
        private Literal failureReason;
        private String failureMsg;

        public ExecuteAction(Literal ac, Intention i)
        {
            action = ac;
            intention = i;
            result = false;
        }

        public override bool Equals(object ao)
        {
            if (ao == null)
            {
                return false;
            } 
            if(!(ao.GetType() == typeof(ExecuteAction)))
            {
                return false;
            }
            ExecuteAction a = (ExecuteAction)ao;
            return action.Equals(a.action);
        }

        public override int GetHashCode()
        {
            return action.GetHashCode();
        }

        public Structure GetActionTerm()
        {
            if (action.GetType() == typeof(Structure))
                return (Structure)action;
            else
                return new Structure(action);
        }

        public Intention GetIntention()
        {
            return intention;
        }

        public bool GetResult()
        {
            return result;
        }

        public void SetResult(bool ok)
        {
            result = ok;
        }

        public void SetFailureReason(Literal reason, String msg)
        {
            failureReason = reason;
            failureMsg = msg;
        }

        public string GetFailureMsg()
        {
            return failureMsg;
        }

        public Literal GetFailureReason()
        {
            return failureReason;
        }

        public override String ToString()
        {
            return "<" + action + "," + intention + "," + result + ">";
        }

        public ExecuteAction Clone()
        {
            ExecuteAction ae = new ExecuteAction((Pred)action.Clone(), intention.Clone());
            ae.result = result;
            return ae;
        }
    }
}