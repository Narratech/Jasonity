using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Code.BDIAgent;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;

namespace Assets.Code.AsSyntax
{
    public class PlanBodyImpl : Structure, IPlanBody, IEnumerable<IPlanBody>
    {
        private ITerm term = null;
        private IPlanBody next = null;
        private BodyType formType = BodyType.none;

        private bool isTerm = false;    // True when the plan body is used as a term instead of an element of a plan

        // Constructor for empty plan body
        public PlanBodyImpl(): this(BodyType.none)
        {
        }

        public PlanBodyImpl(BodyType t):base(t.ToString(), 0)
        {
            formType = t;
        }

        public PlanBodyImpl(BodyType t, bool planTerm):this(t)
        {
            SetAsBodyTerm(planTerm);
        }

        public PlanBodyImpl(BodyType t, ITerm b):this(t, b, false)
        {
        }

        public PlanBodyImpl(BodyType t, ITerm b, bool planTerm):this(t, planTerm)
        {
            formType = t;
            if (b != null)
            {
                srcInfo = b.GetSrcInfo();
            }
            term = b;
        }

        public void SetBodyNext(IPlanBody next) => this.next = next;

        public IPlanBody GetBodyNext() => next;

        public bool IsEmptyBody() => term == null;

        public BodyType GetBodyType() => formType;

        public void SetBodyType(BodyType bt) => formType = bt;

        public ITerm GetBodyTerm() => term;

        public IPlanBody GetHead() => new PlanBodyImpl(GetBodyType(), GetBodyTerm());

        public void SetBodyTerm(ITerm t) => term = t;

        public bool IsBodyTerm() => isTerm;

        public override bool IsAtom() => false;

        public void SetAsBodyTerm(bool b)
        {
            if (b != isTerm)
            {
                isTerm = b;
                if (GetBodyNext() != null) // Only if changed
                {
                    GetBodyNext().SetAsBodyTerm(b);
                }
            }
        }

        public override bool IsPlanBody() => true;

        public IEnumerator<IPlanBody> GetEnumerator()
        {
            return new IEnumeratorEnumerator(this);
        }

        // Overrides some structure methods to work with unification/equals
        public override int GetArity()
        {
            if (term == null)
            {
                return 0;
            }
            else
            {
                return next == null ? 1 : 2;
            }
        }

        public override ITerm GetTerm(int i)
        {
            if (i == 0)
            {
                return term;
            }
            if (i == 1)
            {
                return next;
            }
            return null;
        }

        public override void SetTerm(int i, ITerm t)
        {
            if (i == 0) term = t;
            if (i == 1)
            {
                if (next != null && next.GetBodyTerm().IsVar() && next.GetBodyNext() == null)
                {
                    next.SetBodyTerm(t);
                }
                else
                {
                    Console.WriteLine("Should not SetTerm(1) of body literal!");
                }
            }
        }

        public override IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
        {
            return LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
        }

        override public bool Equals(object o)
        {
            if (o == null) return false;
            if (o == this) return true;

            if (o.GetType() == typeof(IPlanBody))
            {
                IPlanBody b = (IPlanBody)o;
                return formType == b.GetBodyType() && base.Equals(o);
            }
            return false;
        }

        public override int CalcHashCode()
        {
            return formType.GetHashCode() + base.CalcHashCode(); // ???
        }


        // Clones the plan body and adds it at the end of this plan
        public bool Add(IPlanBody bl)
        {
            if (bl == null)
            {
                return true;
            }
            if (term == null)
            {
                bl = bl.ClonePB();
                Swap(bl); // ???
                next = bl.GetBodyNext();
            }
            else if (next == null)
            {
                next = bl;
            }
            else
            {
                next.Add(bl);
            }
            return true;
        }

        public IPlanBody GetLastBody() => next == null ? (this) : next.GetLastBody();

        public bool Add(int index, IPlanBody bl)
        {
            if (index == 0)
            {
                IPlanBody newpb = new PlanBodyImpl(formType, term);
                newpb.SetBodyNext(next);
                Swap(bl);
                next = bl.GetBodyNext();
                GetLastBody().SetBodyNext(newpb);
            }
            else if (next != null)
            {
                next.Add(index - 1, bl);
            }
            else
            {
                next = bl;
            }
            return true;
        }

        public ITerm RemoveBody(int index)
        {
            if (index == 0)
            {
                ITerm oldvalue = term;
                if (next == null)
                {
                    term = null;    // Becomes empty
                }
                else
                {
                    Swap(next);     // Gets values of text
                    next = next.GetBodyNext();
                }
                return oldvalue;
            }
            else
            {
                return next.RemoveBody(index - 1);
            }
        }

        public int GetPlanSize()
        {
            if (term == null)
            {
                return 0;
            }
            else
            {
                return next == null ? 1 : next.GetPlanSize() + 1;
            }
        }

        private void Swap(IPlanBody bl)
        {
            BodyType bt = formType;
            formType = bl.GetBodyType();
            bl.SetBodyType(bt);

            ITerm l = term;
            term = bl.GetBodyTerm();
            bl.SetBodyTerm(l);
        }

        public override ITerm Capply(Unifier u)
        {
            PlanBodyImpl c;
            if (term == null)
            {
                c = new PlanBodyImpl();
                c.isTerm = isTerm;
            }
            else
            {
                c = new PlanBodyImpl(formType, term.Capply(u), isTerm);
                if (c.term.IsPlanBody())
                {
                    c.formType = ((IPlanBody)c.term).GetBodyType();
                    c.next = ((IPlanBody)c.term).GetBodyNext();
                    c.term = ((IPlanBody)c.term).GetBodyTerm();
                }
            }

            if (next != null)
            {
                c.Add((IPlanBody)next.Capply(u));
            }

            return c;
        }

        public new IPlanBody Clone()
        {
            PlanBodyImpl c = term == null ? new PlanBodyImpl() : new PlanBodyImpl(formType, term.Clone(), isTerm);
            c.isTerm = isTerm;
            if (next != null)
            {
                c.SetBodyNext(GetBodyNext().ClonePB());
            }
            return c;
        }

        public IPlanBody ClonePB() => Clone();

        public new ITerm CloneNS(Atom Newnamespace)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            if (term == null)
            {
                return isTerm ? "{ }" : "";
            }
            else
            {
                StringBuilder outs = new StringBuilder();
                if (isTerm)
                {
                    outs.Append("{ ");
                }
                IPlanBody pb = this;
                while (pb != null)
                {
                    if (pb.GetBodyTerm() != null)
                    {
                        outs.Append(pb.GetBodyType());
                        outs.Append(pb.GetBodyTerm());
                    }
                    pb = pb.GetBodyNext();
                    if (pb != null)
                    {
                        outs.Append("; ");
                    }
                }
                if (isTerm)
                {
                    outs.Append(" }");
                }
                return outs.ToString();
            }
        }

        // Automatically generated functions
        // I don't think we need them, so they're not implemented
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public void CountVars(Dictionary<VarTerm, int?> c)
        {
            throw new NotImplementedException();
        }

        public VarTerm GetCyclicVar()
        {
            throw new NotImplementedException();
        }

        public SourceInfo GetSrcInfo()
        {
            throw new NotImplementedException();
        }

        public override bool HasVar(VarTerm t, Unifier u)
        {
            throw new NotImplementedException();
        }

        public override bool IsArithExpr()
        {
            return false;
        }

        public override bool IsCyclicTerm()
        {
            return false;
        }

        public override bool IsGround()
        {
            return false;
        }

        public override bool IsInternalAction()
        {
            return false;
        }

        public override bool IsList()
        {
            return false;
        }

        public override bool IsLiteral()
        {
            return false;
        }

        public override bool IsNumeric()
        {
            return false;
        }

        public override bool IsPred()
        {
            return false;
        }

        public override bool IsRule()
        {
            return false;
        }

        public override bool IsString()
        {
            return false;
        }

        public override bool IsStructure()
        {
            return false;
        }

        public override bool IsUnnamedVar()
        {
            return false;
        }

        public override bool IsVar()
        {
            return false;
        }

        public void SetSrcInfo(SourceInfo s)
        {
            throw new NotImplementedException();
        }

        public bool Subsumes(ITerm l)
        {
            throw new NotImplementedException();
        }

      

        ITerm ITerm.Clone()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private class IEnumeratorEnumerator : IEnumerator<IPlanBody>
        {
            public IEnumeratorEnumerator(PlanBodyImpl pbi)
            {
                Current = pbi;
            }

            public IPlanBody Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext() => Current != null && Current.GetBodyTerm() != null;

            public IPlanBody Next()
            {
                IPlanBody r = Current;
                if (Current != null)
                    Current = Current.GetBodyNext();
                return r;
            }

            public void Reset() { }
        }
    }
}

