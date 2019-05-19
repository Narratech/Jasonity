using System;
using System.Collections.Generic;

using Assets.Code.BDIAgent;
using Assets.Code.functions;
using Assets.Code.ReasoningCycle;
using UnityEngine;

namespace Assets.Code.AsSyntax
{
    public class ArithFunctionTerm:Structure, INumberTerm
    {
        protected INumberTerm value = null;
        private ArithFunction function = null;
        private Agent agent = null;

        public ArithFunctionTerm(ArithFunction function):base(function.GetName(), 2)
        {
            this.function = function; 
        }

        public ArithFunctionTerm(ArithFunctionTerm af) : base(af)
        {
            function = af.function;
            agent = af.agent;
        }

        public ArithFunctionTerm(string functor, int arity):base(functor, arity)
        {

        }

        public override bool IsNumeric()
        {
            return true;
        }

        public override bool IsAtom()
        {
            return false;
        }

        public override bool IsStructure()
        {
            return false;
        }

        public override bool IsLiteral()
        {
            return false;
        }

        public override bool IsArithExpr()
        {
            return true;
        }

        public void SetAgent(Agent ag)
        {
            agent = ag;
        }

        public Agent GetAgent()
        {
            return agent;
        }

        /** computes the value for this arithmetic function (as defined in the NumberTerm interface) */
        public override ITerm Capply(Unifier u)
        {
            if(function == null)
            {
                Debug.Log(GetErrorMsg() + " -- the function cannot be evaluated, it has no function assigned to it!");
            }
            else
            {
                ITerm v = base.Capply(u);
                if (function.AllowUngroundTerms() || v.IsGround())
                {
                    try
                    {
                        value = new NumberTermImpl(function.Evaluate((agent == null ? null : agent.GetReasoner()), ((Literal)v).GetTermsArray()));
                        return value;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(GetErrorMsg()+ " -- error in evaluated!");
                    }
                    finally
                    {
                        //Ignore and return this
                    }
                }
            }
            return Clone();
        }

        public double Solve()
        {
            if (value == null) //try to solve without unifier
                Capply(null);
            if (value == null)
                throw new Exception("Error evaluating " + this + "." + (IsGround() ? "" : " It is not ground."));
            else
                return value.Solve();
        }

        public IEnumerator<Unifier> LogicalConsecuence(Agent ag, Unifier un)
        {
            Debug.Log("Arithmetic term cannot be used for logical consequence");
            return LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            return base.Equals(o);
        }

        public override int CompareTo(ITerm t)
        {
            if (t.GetType() == typeof(VarTerm))
                return t.CompareTo(this) * -1;
            return base.CompareTo(t);
        }

        public override string GetErrorMsg()
        {
            return "Error in '" + this + "' (" + base.GetErrorMsg() + ")";
        }

        public override ITerm Clone()
        {
            return new ArithFunctionTerm(this);
        }
    }
}
