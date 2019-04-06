using System;
using System.Collections.Generic;
using System.Text;
using Assets.Code.Logic;
using Assets.Code.Logic.AsSyntax;
using Assets.Code.ReasoningCycle;

public class PlanBodyImpl : PlanBody
{
    private Term term = null;
    private PlanBody next = null;
    private BodyType formType = BodyType.none;

    private bool isTerm = false;    // True when the plan body is used as a term instead of an element of a plan

    // Constructor for empty plan body
    public PlanBodyImpl()
    {
        this(BodyType.none); // ???
    }

    public PlanBodyImpl(BodyType t)
    {
        // super(t.name(), 0);
        formType = t;
    }

    public PlanBodyImpl(BodyType t, bool planTerm)
    {
        this(t); // ???
        SetAsBodyTerm(planTerm);
    }

    public PlanBodyImpl(BodyType t, Term b)
    {
        this(t, b, false); // ???
    }

    public PlanBodyImpl(BodyType t, Term b, bool planTerm)
    {
        this(t, planTerm); // ???
        formType = t;
        if (b != null)
        {
            // srcInfo = b.getSrcInfo();
        }
        term = b;
    }

    public void SetBodyNext(PlanBody next) => this.next = next;

    public PlanBody GetBodyNext() => next;

    public bool IsEmptyBody() => term == null;

    public BodyType GetBodyType() => formType;

    public void SetBodyType(BodyType bt) => formType = bt;

    public Term GetBodyTerm() => term;

    public PlanBody GetHead() => new PlanBodyImpl(GetBodyType(), GetBodyTerm());

    public void SetBodyTerm(Term t) => term = t;

    public bool IsBodyTerm() => isTerm;

    public bool IsAtom() => false;

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

    public bool IsPlanBody() => true;

    // public Iterator<PlanBody> iterator() { }

    // Overrides some structure methods to work with unification/equals
    public int GetArity()
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

    public Term GetTerm(int i)
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

    public void SetTerm(int i, Term t)
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

    // public Iterator<Unifier> logicalConsequence(Agent ag, Unifier un) { }
    
    override public bool Equals(object o)
    {
        if (o == null) return false;
        if (o == this) return true;

        if (o.GetType() == typeof(PlanBody))
        {
            PlanBody b = (PlanBody)o;
            return formType = b.GetBodyType() && super.Equals(o); // ???
        }
        return false;
    }

    public int CalcHashCode()
    {
        return formType.GetHashCode() + super.CalcHashCode(); // ???
    }


    // Clones the plan body and adds it at the end of this plan
    public bool Add(PlanBody bl)
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

    public PlanBody GetLastBody() => next == null ? (this) : next.GetLastBody();

    public bool Add(int index, PlanBody bl)
    {
        if (index == 0)
        {
            PlanBody newpb = new PlanBodyImpl(formType, term);
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

    public Term RemoveBody(int index)
    {
        if (index == 0)
        {
            Term oldvalue = term;
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

    private void Swap(PlanBody bl)
    {
        BodyType bt = formType;
        formType = bl.GetBodyType();
        bl.SetBodyType(bt);

        Term l = term;
        term = bl.GetBodyTerm();
        bl.SetBodyTerm(l);
    }

    public Term Capply(Unifier u)
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
                c.formType = ((PlanBody)c.term).GetBodyType();
                c.next = ((PlanBody)c.term).GetBodyNext();
                c.term = ((PlanBody)c.term).GetBodyTerm();
            }
        }

        if (next != null)
        {
            c.Add((PlanBody)next.Capply(u));
        }

        return c;
    }

    public PlanBody Clone()
    {
        PlanBodyImpl c = term == null ? new PlanBodyImpl() : new PlanBodyImpl(formType, term.Clone(), isTerm);
        c.isTerm = isTerm;
        if (next != null)
        {
            c.SetBodyNext(GetBodyNext().ClonePB());
        }
        return c;
    }

    public PlanBody ClonePB() => Clone();

    public Term CloneNS(Atom Newnamespace)
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
            if (isTerm) {
                outs.Append("{ ");
            }
            PlanBody pb = this;
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

    public bool HasVar(VarTerm t, Unifier u)
    {
        throw new NotImplementedException();
    }

    public bool IsArithExpr()
    {
        throw new NotImplementedException();
    }

    public bool IsCyclicTerm()
    {
        throw new NotImplementedException();
    }

    public bool IsGround()
    {
        throw new NotImplementedException();
    }

    public bool IsInternalAction()
    {
        throw new NotImplementedException();
    }

    public bool IsList()
    {
        throw new NotImplementedException();
    }

    public bool IsLiteral()
    {
        throw new NotImplementedException();
    }

    public bool IsNumeric()
    {
        throw new NotImplementedException();
    }

    public bool IsPred()
    {
        throw new NotImplementedException();
    }

    public bool IsRule()
    {
        throw new NotImplementedException();
    }

    public bool IsString()
    {
        throw new NotImplementedException();
    }

    public bool IsStructure()
    {
        throw new NotImplementedException();
    }

    public bool IsUnnamedVar()
    {
        throw new NotImplementedException();
    }

    public bool IsVar()
    {
        throw new NotImplementedException();
    }

    public void SetSrcInfo(SourceInfo s)
    {
        throw new NotImplementedException();
    }

    public bool Subsumes(Term l)
    {
        throw new NotImplementedException();
    }

    object ICloneable.Clone()
    {
        throw new NotImplementedException();
    }

    Term Term.Clone()
    {
        throw new NotImplementedException();
    }
}
