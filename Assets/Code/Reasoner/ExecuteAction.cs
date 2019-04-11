// Executes BDI
// Will use other BDIManager classes to handle BB, plans, desires, events, etc.
using System;
using Assets.Code.AsSyntax;
using Assets.Code.Logic;
using BDIManager.Intentions;

namespace BDIMaAssets.Code.ReasoningCycle
{
    public class ExecuteAction
    {
        private Literal body;
        private Intention curInt;

        public ExecuteAction(Literal body, Intention curInt)
        {
            this.body = body;
            this.curInt = curInt;
        }

        internal Intention GetIntention()
        {
            throw new NotImplementedException();
        }

        internal object GetActionTerm()
        {
            throw new NotImplementedException();
        }

        internal bool GetResult()
        {
            throw new NotImplementedException();
        }

        internal string GetFailureMsg()
        {
            throw new NotImplementedException();
        }

        internal object GetFailureReason()
        {
            throw new NotImplementedException();
        }
    }
}