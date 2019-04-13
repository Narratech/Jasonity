using System;
using System.IO;
using Assets.Code.ReasoningCycle;

namespace Assets.Code.AsSyntax
{
    public class Plan : Structure
    {
        private ITerm TAtomic = AsSyntax.CreateAtom("atomic");   // ???
        private ITerm TBreakPoint = AsSyntax.CreateAtom("breakpoint"); // ???
        private ITerm TAllUnifs = AsSyntax.CreateAtom("all_unifs"); // ???

        private Pred label = null;
        private Trigger tevent = null;
        private ILogicalFormula context;
        private IPlanBody body;

        private bool isAtomic = false;
        private bool isAllUnifs = false;
        private bool hasBreakpoint = false;

        private bool isTerm = false;    // True when the plan body is used as a term instead of an element of a plan

        private string source = ""; // Source of this plan (file, url, etc.)

        public Plan():base("plan", 0)
        {
        }

        public Plan(Pred label, Trigger te, ILogicalFormula ct, IPlanBody bd):base("plan", 0)
        {
            tevent = te;
            tevent.SetAsTriggerTerm(false);
            SetLabel(label);
            SetContext(ct);
            if (bd == null)
            {
                body = new PlanBodyImpl();
            }
            else
            {
                body = bd;
                body.SetAsBodyTerm(false);
            }
        }

        public override int GetArity() => 4;

        public void SetSource(string f)
        {
            if (f != null)
            {
                source = f;
            }
        }

        public string GetSource() => source;

        private ITerm noLabelAtom = new Atom("nolabel");

        public override ITerm GetTerm(int i)
        {
            switch (i)
            {
                case 0:
                    return (label == null) ? noLabelAtom : label;
                case 1:
                    return tevent;
                case 2:
                    return (context == null) ? Literal.LTrue : context;
                case 3:
                    if (body.GetBodyNext() == null && body.GetBodyTerm().IsVar())
                    {
                        return body.GetBodyTerm();
                    }
                    return body;
                default:
                    return null;
            }
        }

        public override void SetTerm(int i, ITerm t)
        {
            switch (i)
            {
                case 0:
                    label = (Pred)t;
                    break;
                case 1:
                    tevent = (Trigger)t;
                    break;
                case 2:
                    context = (ILogicalFormula)t;
                    break;
                case 3:
                    body = (IPlanBody)t;
                    break;
            }
        }

        public void SetLabel(Pred p)
        {
            label = p;
            isAtomic = false;
            hasBreakpoint = false;
            isAllUnifs = false;
            if (p != null && p.HasAnnot())
            {
                foreach (ITerm t in label.GetAnnots())
                {
                    if (t.Equals(TAtomic))
                    {
                        isAtomic = true;
                    }
                    if (t.Equals(TBreakPoint))
                    {
                        hasBreakpoint = true;
                    }
                    if (t.Equals(TAllUnifs))
                    {
                        isAllUnifs = true;
                    }
                }
            }
        }

        public Pred GetLabel() => label;

        public void DelLabel() => SetLabel(null);

        public void SetContext(ILogicalFormula le)
        {
            context = le;
            if (Literal.LTrue.Equals(le))
            {
                context = null;
            }
        }

        public void SetAsPlanTerm(bool b) => isTerm = b;

        public bool IsPlanTerm() => isTerm;

        public IListTerm GetAsListOfTerms()
        {
            IListTerm l = new ListTermImpl();
            l.Add(GetLabel());
            l.Add(GetTrigger());
            l.Add(GetContext());
            l.Add(GetBody());
            return l;
        }

        public object GetSrcInfo()
        {
            throw new NotImplementedException();
        }

        // Creates a plan from a list with four elements: [Literal, Trigger, Context, Body]
        public new Plan NewFromListOfTerms(IListTerm lt)
        {
            ITerm c = lt[2];
            if (c.IsPlanBody())
            {
                c = ((IPlanBody)c).GetBodyTerm();
            }
            return new Plan(new Pred((Literal)(lt[0])), (Trigger)lt[1], (ILogicalFormula)c, (IPlanBody)lt[3]);
        }

        public Trigger GetTrigger() => tevent;

        public ILogicalFormula GetContext() => context;

        public IPlanBody GetBody() => body;

        internal bool IsAtomic() => isAtomic;

        public bool HasBreakpoint() => hasBreakpoint;

        public bool IsAllUnifs() => isAllUnifs;

        // Returns an unifier if this plan is relevant for event "te", null otherwise
        internal Unifier IsRelevant(Trigger te)
        {
            // Annots in plan's TE must be a subset of the ones in the event!
            Unifier u = new Unifier();
            if (u.UnifiesNoUndo(tevent, te))
            {
                return u;
            }
            else
            {
                return null;
            }
        }

        // bool Equals(object o);

        public new Plan Capply(Unifier u)
        {
            Plan p = new Plan();
            if (label != null)
            {
                p.label = (Pred)label.Capply(u);
                p.isAtomic = isAtomic;
                p.hasBreakpoint = hasBreakpoint;
                p.isAllUnifs = isAllUnifs;
            }

            p.tevent = tevent.Capply(u);
            if (context != null)
            {
                p.context = (ILogicalFormula)context.Capply(u);
            }
            p.body = (IPlanBody)body.Capply(u);
            p.SetSrcInfo(srcInfo); // ???
            p.isTerm = isTerm;
            return p;
        }

        public override ITerm Clone()
        {
            Plan p = new Plan();
            if (label != null)
            {
                p.SetLabel((Pred)label.Clone());
            }
            p.tevent = tevent.Clone();
            if (context != null)
            {
                p.context = (ILogicalFormula)context.Clone();
            }
            p.body = body.ClonePB();
            p.SetSrcInfo(srcInfo); // ???
            p.isTerm = isTerm;

            return p;
        }

        public new Plan CloneNS(Atom ns) => (Plan)Clone();

        public Plan CloneOnlyBody()
        {
            Plan p = new Plan();
            if (label != null)
            {
                p.label = label;
                p.isAtomic = isAtomic;
                p.hasBreakpoint = hasBreakpoint;
                p.isAllUnifs = isAllUnifs;
            }

            p.tevent = tevent.Clone();
            p.context = context;
            p.body = body.ClonePB();

            p.SetSrcInfo(srcInfo); 
            p.isTerm = isTerm;

            return p;
        }

        public override string ToString() => ToASSTring();

        private string ToASSTring()
        {
            string b, e;
            if (isTerm)
            {
                b = "{ ";
                e = " }";
            }
            else
            {
                b = "";
                e = ".";
            }
            return b + ((label == null) ? "" : "@" + label + " ") + tevent + ((context == null) ? "" : " : " + context)
                + (body.IsEmptyBody() ? "" : " <- " + body) + e;
        }
    }
}
