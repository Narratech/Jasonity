using .Assets.Code.Logic.Parser;
using Assets.Code.Logic.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    class Pred : Structure
    {
        private ListTerm annotations;

        public Pred(string functor) : base(functor)
        {

        }

        public Pred(Literal l)
        {
            this(l.GetNS(), l); //This has to call the other constructor that uses namespaces and literals but i dont know how
        }

        public Pred(Atom namespce, String functor) : base(namespce, functor)
        {

        }

        public Pred(Atom namespce, Literal l): base(namespce, l)
        {
            if (l.HasAnnot())
            {
                annotations = l.GetAnnots().CloneLT();
            } else
            {
                annotations = null;
            }
        }

        private Pred(Literal l, Unifier u) : base(l, u)
        {
            if (l.HasAnnot())
            {
                SetAnnots((ListTerm)l.GetAnnots().Capply(u));
            } else
            {
                annotations = null;
            }
        }

        public Pred(string functor, int termSize) : base(functor, termSize)
        {

        }

        public static Pred ParsePred(string spread)
        {
            as2j parser = new as2j(new StringReader(spread)); //I need an equivalent of StringReader
            try
            {
                return parser.Pred();
            } catch (Exception e)
            {
                //logger.log(Level.SEVERE, "Error parsing predicate " + spread, e);
                return null;
            }
        }

        public override bool IsPred()
        {
            return true;
        }

        public override bool IsAtom()
        {
            return base.IsAtom() && !HasAnnot();
        }

        public override bool IsGround()
        {
            if (annotations == null)
            {
                return base.IsGround();
            } else
            {
                return base.IsGround() && annotations.IsGround();
            }
        }

        private bool HasAnnot()
        {
            throw new NotImplementedException();
        }

        private void SetAnnots(ListTerm listTerm)
        {
            throw new NotImplementedException();
        }
    }
}
