using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class ForkStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new ForkStdLib();
            return singleton;
        }

        override public ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray();
        }

        override public int GetMinArgs()
        {
            return 2;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
        if ( !(args[0].GetType() == typeof(Atom)))
            throw JasonityException.CreateWrongArgument(this,"first argument must be 'and' or 'or'.");
        }

        override public bool SuspendIntention()
        {
            return true;
        }

        override public bool CanBeUsedInContext()
        {
            return false;
        }

        private static readonly Structure joinS = new Structure(".join");

        public static readonly Atom aAnd = new Atom("and");
        public static readonly Atom aOr = new Atom("or");

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            ForkData fd = new ForkData(((Atom)args[0]).Equals(aAnd));

            Intention currentInt = (Intention)ts.GetCircumstance().GetSelectedIntention();
            for (int iPlans = 1; iPlans < args.Length; iPlans++)
            {
                Intention i = new ForkIntention(currentInt, fd);
                fd.AddIntention(i);
                i.Pop(); // remove the top IM, it will be introduced back later (modified)
                IntendedPlan im = (IntendedPlan)currentInt.Peek().Clone();

                // adds the .join in the plan
                InternalActionLiteral joinL = new InternalActionLiteral(joinS, ts.GetAgent());
                joinL.AddTerm(new ObjectTermImpl(fd));
                IPlanBody joinPB = new PlanBodyImpl(BodyType.internalAction, joinL);
                joinPB.SetBodyNext(im.GetCurrentStep().GetBodyNext());

                // adds the argument in the plan (before join)
                IPlanBody whattoadd = (IPlanBody)args[iPlans].Clone();
                whattoadd.Add(joinPB);
                whattoadd.SetAsBodyTerm(false);
                im.InsertAsNextStep(whattoadd);
                im.RemoveCurrentStep(); // remove the .fork
                i.Push(im);
                ts.GetCircumstance().AddRunningIntention(i);
            }
            return true;
        }

        public class ForkData
        {
            public bool isAnd = true;
            public ISet<Intention> intentions = new HashSet<Intention>();
            public int toFinish = 0;

            public ForkData(bool isAnd)
            {
                this.isAnd = isAnd;
            }

            public void AddIntention(Intention i)
            {
                intentions.Add(i);
                toFinish++;
            }

            
            public override string ToString()
            {
                StringBuilder s = new StringBuilder("fork data");
                if (isAnd)
                    s.Append(" (and) ");
                else
                    s.Append(" (or) ");
                s.Append(" intentions = { ");
                foreach (Intention i in intentions)
                {
                    s.Append(" " + i.GetID());
                }
                s.Append(" } waiting for " + toFinish);
                return s.ToString();
            }
        }

        class ForkIntention : Intention
        {
            ForkData fd;
            int forkPoint;

            public ForkIntention(Intention i, ForkData fd)
            {
                i.CopyTo(this);
                forkPoint = i.Size();
                this.fd = fd;
            }

            public override bool DropDesire(Trigger te, Unifier un)
            {
                bool r = base.DropDesire(te, un);
                if (r && Size() < forkPoint)
                {
                    if (fd.toFinish > 0)
                    { // the first intentions of the fork being dropped, keep it and ignore the rest
                        fd.toFinish = 0;
                        return true;
                    }
                    else
                    {
                        ClearPlans();
                        //System.out.println("ignore intention");
                        return false;
                    }
                }
                return r;
            }

            public override void Fail(Circumstance c)
            {
                if (Size() >= forkPoint && fd.isAnd)
                { // the fail is above fork, is an fork and, remove the others
                    foreach (Intention ifo in fd.intentions)
                    {
                        c.DropIntention(ifo);
                    }
                }
            }

            public override KeyValuePair<Event, int> FindEventForFailure(Trigger tevent, PlanLibrary pl, Circumstance c)
            {
                KeyValuePair<Event, int> t = (KeyValuePair<Event, int>)base.FindEventForFailure(tevent, pl, c);
                if (t.Value <= forkPoint)
                {
                    if (fd.isAnd)
                    {
                        fd.intentions.Remove(this);
                        foreach (Intention ifo in fd.intentions)
                        {
                            c.DropIntention(ifo);
                        }
                    }
                    else
                    {
                        return new KeyValuePair<Event, int>(null, t.Value);
                    }
                }
                return t;
            }
        }
    }
}
