using Assets.Code.Agent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.AsSyntax
{
    /*A particular type of Literal used to represent internal actions 
     * (which has a "." in the functor)
     */
    public class InternalActionLiteral:Structure, ILogicalFormula
    {
        private InternalAction ia = null;

        public InternalActionLiteral(string functor):base(functor)
        {

        }

        //used by clone
        public InternalActionLiteral(InternalActionLiteral l):base(l.GetNS(), (Structure)l)
        {
            this.ia = l.ia;
        }

        //used by capply
        private InternalActionLiteral(InternalActionLiteral l, Unifier u):base((Structure)l, u)
        {
            this.ia = l.ia;
        }

        //used by cloneNS
        private InternalActionLiteral(Atom ns, InternalActionLiteral l):base(ns, (Structure)l)
        {
            this.ia = l.ia;
        }

        //used by the parser
        public InternalActionLiteral(Structure p, Agent.Agent ag):this(DefaultNS, p, ag)
        {

        }

        public InternalActionLiteral(Atom ns, Structure p, Agent.Agent ag) : base(ns, p)
        {
            if (ag != null)
                ia = ag.GetIA(GetFunctor());
        }

        public override bool IsInternalAction()
        {
            return true;
        }

        public override bool IsAtom()
        {
            return false;
        }

        public override Literal MakeVarsAnnon(Unifier u)
        {
            Literal t = base.MakeVarsAnnon(u);
            if (t.GetFunctor().Equals(".puts"))
            {
                ((puts)puts.Create()).MakeVarsAnnon(t, u);
            }
            return t;
        }

        public IEnumerator<Unifier> LogicalConsecuence(Agent.Agent ag, Unifier un)
        {
            if (ag == null || ag.GetReasoner().GetUserAgArch().IsRunning())
            {
                try
                {
                    InternalAction ia = GetIA(ag);
                    if (!ia.CanBeUsedInContext())
                    {
                        Debug.Log(GetErrorMsg()+ ": internal action" + GetFunctor() + " cannot be used in context or rules");
                        return LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
                    }
                    //calls IA's execute method
                    object oresult = ia.Execute(ag.GetReasoner(), un, ia.PrepareArguments(this, un));
                    if (oresult.GetType() == typeof(bool) && (bool)oresult)
                    {
                        return LogExpr.CreateUnifIterator(un);
                    }
                    else if (oresult.GetType() == typeof(IEnumerator<>))
                    {
                        return (IEnumerator<Unifier>)oresult;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("*-*-*"+GetFunctor()+" concurrent exception - try later" + e.ToString());
                    try
                    {
                        Thread.Sleep(2000);
                    }
                    catch (Exception e1)
                    {
                        return LogicalConsecuence(ag, un);
                    }
                }
            }
            return LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
        }

        public void SetIA(InternalAction ia)
        {
            this.ia = ia;
        }

        public InternalAction GetIA(Agent.Agent ag)
        {
            if (ia == null && ag != null)
                ia = ag.GetIA(GetFunctor());
            return ia;
        }

        public override string GetErrorMsg()
        {
            string src = GetSrcInfo() == null ? "" : " (" + GetSrcInfo() + ")";
            return "Error in internal action '" + this + "'" + src;
        }

        public override ITerm Capply(Unifier u)
        {
            return new InternalActionLiteral(this, u);
        }

        public new InternalActionLiteral Clone()
        {
            return new InternalActionLiteral(this);
        }

        public override Literal CloneNS(Atom newNamespace)
        {
            return new InternalActionLiteral(newNamespace, this);
        }
    }
}
