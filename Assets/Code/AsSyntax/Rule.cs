using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    public class Rule : LiteralImpl
    {
        private ILogicalFormula body = null;
        private bool isTerm = false;

        public Rule(Literal head, ILogicalFormula body) : base(head)
        {
            if (head.IsRule())
            {
                //logger.log(Level.SEVERE, "The rule head (" + head + ") is a rule!", new Exception());
            } else if (IsInternalAction())
            {
                //logger.log(Level.SEVERE,"The rule head ("+head+") can not be an internal action!", new Exception());
            }
            else if (head == LTrue || head == LFalse)
            {
                //logger.log(Level.SEVERE,"The rule head ("+head+") can not be a true or false!", new Exception());
            }
            this.body = body;
        }

        public Rule(Rule r, Unifier u) : base(r, u)
        {
            isTerm = r.isTerm;
            body = (ILogicalFormula)r.body.Capply(u);
            predicateIndicatorCache = null;
        }

        public override bool IsRule()
        {
            return true;
        }

        public override bool IsAtom()
        {
            return false;
        }

        public override bool IsGround()
        {
            return false;
        }

        public void SetAsTerm(bool b)
        {
            isTerm = b;
        }

        public bool IsTerm()
        {
            return isTerm;
        }

        public override bool Equals(object o)
        {
            if (o != null && o.GetType() == typeof(Rule)) {
                Rule r = (Rule)o;
                return base.Equals(o) && body.Equals(r.body);
            }
            return false;
        }

        public override int CalcHashCode()
        {
            return base.CalcHashCode() + body.GetHashCode();
        }

        public ILogicalFormula GetBody()
        {
            return body;
        }

        public Literal GetHead()
        {
            return new LiteralImpl(this);
        }

        public override Literal MakeVarsAnnon(Unifier un)
        {
            if (body.GetType() == typeof(Literal))
            {
                ((Literal)body).MakeVarsAnnon(un);
            }
            return base.MakeVarsAnnon(un);
        }

        public override ITerm Capply(Unifier u)
        {
            return new Rule(this, u);
        }

        public new Rule Clone()
        {
            Rule r = new Rule((Literal)base.Clone(), (ILogicalFormula)body.Clone());
            r.predicateIndicatorCache = null;
            r.ResetHashCodeCache();
            r.isTerm = isTerm;
            return r;
        }

        public Literal HeadClone()
        {
            return (Literal)base.Clone();
        }

        public Literal HeadCApply(Unifier u)
        {
            return (Literal)base.Capply(u);
        }

        public override string ToString()
        {
            if (IsTerm())
            {
                return "{ " + base.ToString() + " :- " + body + " }";
            }
            else
            {
                return base.ToString() + " :- " + body;
            }
        }

        public override bool HasVar(VarTerm t, Unifier u)
        {
            if (base.HasVar(t, u))
            {
                return true;
            }
            return body.HasVar(t, u);
        }

        public override void CountVars(Dictionary<VarTerm, int?> c)
        {
            base.CountVars(c);
            body.CountVars(c);
        }
    }
}