using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.Logic;

/**
 * This class is used to unify the selected event and the plan in order to get the relevant plans 
 */
namespace Assets.Code.ReasoningCycle
{
    class Unifier
    {
        Dictionary<VarTerm, Term> function = new Dictionary<VarTerm, Term>();

        public bool Remove(VarTerm v)
        {
            return function.Remove(v);
        }

        public bool Unifies(Term term, Literal topLiteral)
        {
            throw new NotImplementedException();
        }

        // Undoes the variables' mapping if unification fails
        public bool Unifies(Term term, Term term2)
        {
            Dictionary<VarTerm, Term> cFunction = function;
            if (UnifiesNoUndo(term, term2))
            {
                return true;
            }
            else
            {
                function = cFunction;
                return false;
            }
        }

        private bool UnifiesNoUndo(Term term, Term term2)
        {
            if (term == null && term2 == null) return true;
            if (term == null && term2 != null) return false;
            if (term != null && term2 == null) return false;

            Pred np1 = null;
            Pred np2 = null;

            if (np1.GetType() == typeof(Pred) && np2.GetType() == typeof(Pred))
            {
                np1 = (Pred)term;
                np2 = (Pred)term2;

                // tests when np1 or np2 are variables with annotations
                if (np1.IsVar() && np1.HasAnnot() || np2.IsVar() && np2.HasAnnot())
                {
                    if (!np1.HasSubsetAnnot(np2, this))
                    {
                        return false;
                    }
                }
            }

            if (term.IsCyclicTerm() && term2.IsCyclicTerm()) // Both are cycled terms
            {
                // Unification of cyclic terms: Remove variables to avoid loops and test the structure, then reintroduce vars
                VarTerm v1 = term.GetCyclicVar();
                VarTerm v2 = term2.GetCyclicVar();
                Remove(v1);
                Remove(v2);
                try
                {
                    return UnifiesNoUndo(new LiteralImpl((Literal)term), new LiteralImpl((Literal)term2); // ???
                }
                finally
                {
                    function.Add(v1, term);
                    function.Add(v2, term2);
                }
            }
            else
            {
                if (term.IsCyclicTerm() && Get(term.GetCyclicVar()) == null) // Reintroduce cycles in the unifier
                {
                    function.Add(term.GetCyclicVar(), term);
                }
                if (term2.IsCyclicTerm() && Get(term.GetCyclicVar()) == null)
                {
                    function.Add(term2.GetCyclicVar(), term2);
                }
            }

            // Unify as Term
            bool ok = UnifyTerms(term, term2);

            // If term is a unified variable, clear its annotations
            if (ok && term != null) // Both are predicates
            {
                if (term.IsVar() && term.HasAnnot())
                {
                    term = Deref((VarTerm)term); // ???
                    Term termvl = function[(VarTerm)term];
                    if (termvl != null && termvl.IsPred())
                    {
                        Pred pvl = termvl.Clone();
                        pvl.ClearAnnots();
                        Bind((VarTerm)term, pvl);
                    }
                }
                if (term2.IsVar() && term2.HasAnnot())
                {
                    term2 = Deref((VarTerm)term2);
                    Term term2vl = function[(VarTerm)term2];
                    if (term2vl != null && term2vl.IsPred())
                    {
                        Pred pvl = term2vl.Clone();
                        pvl.ClearAnnots();
                        Bind((VarTerm)term2, pvl);
                    }
                }
            }
            return ok;
        }

        internal bool Unifies(Trigger t, Trigger trigger)
        {
            return t.GetType() == trigger.GetType() && Unifies(t.GetLiteral(), trigger.GetLiteral());
        }

        private bool Bind(VarTerm term, Pred pvl)
        {
            throw new NotImplementedException();
        }

        private VarTerm Deref(VarTerm term)
        {
            Term vl = function[term];
            VarTerm first = term;
            while (vl != null && term.IsVar())
            {
                term = (VarTerm)vl;
                vl = function[term];
            }
            if (first != term)
            {
                function.Add(first, vl);
            }
            return term;
        }

        private bool UnifyTerms(Term term, Term term2)
        {
            throw new NotImplementedException();
        }

        private object Get(VarTerm var)
        {
            Term vl = function[var];
            if (vl != null && vl.IsVar())
            {
                return Get((VarTerm)vl);
            }
            return vl;
        }

        public bool Unifies(Literal literal, Literal topLiteral) // ???
        {
            throw new NotImplementedException();
        }

        Dictionary<VarTerm,Term> GetFunction()
        {
            return function;
        }
    }
}
