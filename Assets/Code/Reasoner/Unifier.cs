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

        public bool Unifies(Term term, Literal topLiteral) // ???
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

        private bool Bind(VarTerm term, Pred pvl) // This one is weird because I need it but it doesn't exist in the original
        {
            throw new NotImplementedException();
        }

        private VarTerm GetVarForUnifier(VarTerm term)
        {
            term = Deref(term).CloneNS(Literal.DefaultNS);
            term.SetNegated(Literal.LPos);
            return term;
        }

        private bool UnifiesNamespace(VarTerm term, Literal lpvl) // This one is weird too, original uses two Literals, I'm confused
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
            if (term.IsArithExpr())
            {
                term = term.Capply(this);
            }
            if (term2.IsArithExpr())
            {
                term2 = term2.Capply(this);
            }

            bool termIsVar = term.IsVar();
            bool term2IsVar = term2.IsVar();

            // One of them is a variable
            if (termIsVar || term2IsVar)
            {
                VarTerm termv = termIsVar ? (VarTerm)term : null;
                VarTerm term2v = term2IsVar ? (VarTerm)term2 : null;

                // Get their values
                Term termvl = termIsVar ? Get(termv) : term; // ???
                Term term2vl = term2IsVar ? Get(term2v) : term2; // ???

                if (termvl != null && term2vl != null) // Unify the values of the two variables
                {
                    return UnifiesNoUndo(termvl, term2vl);
                }
                else if (termvl != null) // Unify a variable with a value
                {
                    return Bind(term2v, termvl);
                }
                else if (term2vl != null)
                {
                    return Bind(termv, term2vl);
                }
                else // Unify two variables
                {
                    if (!UnifyTerms(termv.GetNS(), term2v.GetNS()))
                    {
                        return false;
                    }
                    if (termv.Negated() != term2v.Negated())
                    {
                        return false;
                    }
                    Bind(termv, term2v);
                    return true;
                }
            }
        }

        private void Bind(VarTerm vt1, VarTerm vt2)
        {
            vt1 = GetVarForUnifier(vt1);
            vt2 = GetVarForUnifier(vt2);
            int compare = vt1.CompareTo(vt2);
            if (compare < 0)
            {
                function.Add(vt1, (Term)vt2);
            }
            else if (compare > 0)
            {
                function.Add(vt2, (Term)vt1);
            }
            // Doesn't bind if (compare == 0), because they are the same
        }

        private bool UnifyTerms(VarTerm varTerm1, VarTerm varTerm2) // Yet again, something that doesn't exist! I hate this!
        {
            throw new NotImplementedException();
        }

        private bool Bind(VarTerm vt, Term term)
        {
            if (vt.Negated()) // Negated variables unify only with negated literals
            {
                if (!term.IsLiteral() || !((Literal)term).Negated())
                {
                    return false;
                }
                term = (Literal)term.Clone(); // I don't understand this cast, but the original code does it, so I don't know
                ((Literal)term).SetNegated(Literal.LPos);
            }

            // Namespace
            if (term.IsLiteral())
            {
                Literal lterm = (Literal)term;
                if (!UnifiesNamespace(vt, lterm))
                {
                    return false;
                }
                if (lterm.GetNS() != Literal.DefaultNS)
                {
                    term = (Term)lterm.CloneNS(Literal.DefaultNS);
                }
            }
            if (!term.IsCyclicTerm() && term.HasVar(vt, this))
            {
                term = new CyclicTerm((Literal)term, vt.Clone());
            }
            function.Add(GetVarForUnifier(vt), term);
            return true;
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

        public bool Unifies(Literal literal, Literal topLiteral) // Why does this exist? Why?!
        {
            throw new NotImplementedException();
        }

        Dictionary<VarTerm,Term> GetFunction()
        {
            return function;
        }
    }
}
