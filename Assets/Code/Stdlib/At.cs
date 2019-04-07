using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class At: DefaultInternalAction
    {
        public static readonly string atAtom = ".at";
        
        public int GetMinArgs()
        {
            return 2;
        }

        public int GetMaxArgs()
        {
            return 2;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            IStringTerm time = (IStringTerm)args[0];
            string stime = time.GetString();

            //Parse time
            long deadline = -1;

            //If it start with now
            if (stime.StartsWith("now"))
            {
                //It is something like "now +3 minutes"
                stime = stime.Substring(3).Trim();
                //Get the amount of time
                if (stime.StartsWith("+"))
                {
                    stime = stime.Substring(1).Trim();
                    int pos = stime.IndexOf(" ");
                    if (pos > 0)
                    {
                        deadline = int.Parse(stime.Substring(0, pos));
                        //Get the time unit
                        stime = stime.Substring(pos).Trim();
                        if (stime.Equals("s") || stime.StartsWith("second"))
                        {
                            deadline *= 1000;
                        }
                        if (stime.Equals("m") || stime.StartsWith("minute"))
                        {
                            deadline *= 1000 * 60;
                        }
                        if (stime.Equals("h") || stime.StartsWith("hour"))
                        {
                            deadline *= 1000 * 60 * 60;
                        }
                        if (stime.Equals("d") || stime.StartsWith("day"))
                        {
                            deadline *= 1000 * 60 * 60 * 24;
                        }
                    }
                }
            }
            else
            {
                throw new JasonException("The time parameter ('"+stime+ "') of the internal action 'at' is not implemented!");
            }
            if (deadline == -1)
            {
                throw new JasonException("The time parameter ('" + time + "') of the internal action 'at' did not parse correctly!");
            }

            Trigger te = Trigger.TryToGetGetTrigger(args[1]);

            Agent.Agent.GetScheduler().Schedule(new CheckDeadLine(te, ts), deadline, TimeUnit.MILLISECONDS);
            return true;
        }

        //Here they use AtomicInteger class, but ther is nor such a thing in C#
        private static int idCount = 0;
        ///******************************************************************
        private ConcurrentDictionary<int?, CheckDeadline> ats = new ConcurrentDictionary<int?, CheckDeadline>();

        public void CancelAts()
        {
            foreach(CheckDeadline t in ats.Values)
            {
                t.Cancel();
            }
        }

        class CheckDeadline
        {
            private int id = 0;
            private Event @event;
            private Reasoner ts;
            private bool cancelled = false;

            public CheckDeadline(Trigger te, Reasoner ts)
            {
                this.id = idCount + 1;
                this.@event = new Event(te, Intention.EmptyInt);
                this.ts = ts;
                ats.Put(id, this);
            }

            public void Cancel()
            {
                cancelled = true;
            }

            public void Run()
            {
                try
                {
                    if (!cancelled)
                    {
                        ts.GetC().AddEvent(@event);
                        ts.GetUserAgArch().Wake();
                    }
                }
                finally
                {
                    ats.Remove(id);
                }
            }
        }
    }
}
