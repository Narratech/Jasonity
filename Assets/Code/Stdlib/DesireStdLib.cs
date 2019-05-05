using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class DesireStdLib : IntendStdLib
    {
        enum Step { selEvt, evt, useIntends, end }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return AllDesires(ts.GetCircumstance(), args[0] as Literal, args.Length == 2 ? args[1] : null, un);
        }

        public static IEnumerator<Unifier> AllDesires(Circumstance C, Literal l, ITerm intAsTerm, Unifier un)
        {
            Trigger teFroml = new Trigger(TEOperator.add, TEType.achieve, l);

            return new EnumeratorImpl(C, l, intAsTerm, un, teFroml);
        }

        private class EnumeratorImpl : IEnumerator<Unifier>
        {
            Step curStep;
            Unifier solution; // the current response (which is an unifier)
            IEnumerator<Event> evtIterator;
            IEnumerator<Unifier> intendInterator;
            private Circumstance C;
            private Literal l;
            private ITerm intAsTerm;
            private Unifier un;
            private Trigger teFromL;

            //static EnumeratorImpl()
            //{
            //    Find();
            //}

            public EnumeratorImpl(Circumstance c, Literal l, ITerm intAsTerm, Unifier un, Trigger teFroml)
            {
                this.C = c;
                this.l = l;
                this.intAsTerm = intAsTerm;
                this.un = un;
                curStep = Step.selEvt;
                solution = null;
                evtIterator = null;
                intendInterator = null;
                this.teFromL = teFroml;
            }

            //{ find(); }

            public Unifier Current
            {
                get
                {
                    if (solution == null) Find();
                    Unifier b = solution;
                    Find(); // find next response
                            //logger.info("* try "+b+" for "+teFromL);
                    return b;
                }
            }

            void Find()
            {
                switch (curStep)
                {
                    case Step.selEvt:
                        curStep = Step.evt; // set next step

                        // we need to check the selected event in this cycle (already removed from E)
                        if (C.GetSelectedEvent() != null)
                        {
                            Trigger t = C.GetSelectedEvent().GetTrigger();
                            Intention i = C.GetSelectedEvent().GetIntention();
                            if (i != Intention.emptyInt && !i.IsFinished())
                            {
                                t = (Trigger)t.Capply(i.Peek().GetUnif());
                            }
                            solution = un.Clone();
                            if (solution.UnifiesNoUndo(teFromL, t))
                            {
                                return;
                            }
                        }
                        Find();
                        return;

                    case Step.evt:
                        if (evtIterator == null)
                            evtIterator = C.GetEventsPlusAtomic();

                        if (evtIterator.MoveNext())
                        {
                            Event ei = evtIterator.Current;
                            Trigger t = ei.GetTrigger();
                            Intention i = ei.GetIntention();
                            if (i != Intention.emptyInt && !i.IsFinished())
                            {
                                t = t.Capply(i.Peek().GetUnif());
                            }
                            solution = un.Clone();
                            if (solution.UnifiesNoUndo(teFromL, t))
                            {
                                return;
                            }
                        }
                        else
                        {
                            curStep = Step.useIntends; // set next step
                        }
                        Find();
                        return;

                    case Step.useIntends:
                        if (intendInterator == null)
                            intendInterator = AllIntentions(C, l, intAsTerm, un);

                        if (intendInterator.MoveNext())
                        {
                            solution = intendInterator.Current;
                            return;
                        }
                        else
                        {
                            curStep = Step.end; // set next step
                        }
                        break;

                    case Step.end:
                        break;
                }
                solution = null; //nothing found
            }

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                return solution != null;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
