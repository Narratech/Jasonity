using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;

/**
 * This class is used to unify the selected event and the plan in order to get the relevant plans 
 */
namespace Assets.Code.ReasoningCycle
{
    public class Unifier : IEnumerable<VarTerm>
    {
        Dictionary<VarTerm, ITerm> function = new Dictionary<VarTerm, ITerm>();

        // Gets the value for a variable
        // If it is unified with another variable, gets that value
        public ITerm Get(string var) => Get(new VarTerm(var));

        public bool Remove(VarTerm v) => function.Remove(v);

        public IEnumerator<VarTerm> Enumerator() => function.Keys.GetEnumerator();

        public ITerm Get(VarTerm var)
        {
            ITerm vl;
            function.TryGetValue(var, out vl);
            if (vl != null && vl.IsVar())
            {
                return Get((VarTerm)vl);
            }
            return vl;
        }

        public VarTerm GetVarFromValue(ITerm vl)
        {
            foreach (VarTerm v in function.Keys)
            {
                ITerm vvl;
                function.TryGetValue(v, out vvl);
                if (vvl.Equals(vl)) return v;
            }
            return null;
        }

        public bool Unifies(Trigger te1, Trigger te2) => te1.SameType(te2) && Unifies(te1.GetLiteral(), te2.GetLiteral());

        public bool UnifiesNoUndo(Trigger te1, Trigger te2) => te1.SameType(te2) && UnifiesNoUndo(te1.GetLiteral(), te2.GetLiteral());

        // Undoes the variables' mapping if unification fails
        public bool Unifies(ITerm term, ITerm term2)
        {
            Dictionary<VarTerm, ITerm> cFunction = function;
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

        private Dictionary<VarTerm, ITerm> CloneFunction()
        {
            Dictionary<VarTerm, ITerm> clone = new Dictionary<VarTerm, ITerm>(function);
            return clone;
        }

        public bool UnifiesNoUndo(ITerm term, ITerm term2)
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
                    return UnifiesNoUndo(new LiteralImpl((Literal)term), new LiteralImpl((Literal)term2));
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
                if (term.IsVar() && ((Pred)term).HasAnnot())
                {
                    term = Deref((VarTerm)term); // ???
                    ITerm termvl;
                    function.TryGetValue((VarTerm)term, out termvl);
                    if (termvl != null && termvl.IsPred())
                    {
                        Pred pvl = (Pred)termvl.Clone();
                        pvl.ClearAnnots();
                        Bind((VarTerm)term, pvl);
                    }
                }
                if (term2.IsVar() && ((Pred)term2).HasAnnot())
                {
                    term2 = Deref((VarTerm)term2);
                    ITerm term2vl;
                    function.TryGetValue((VarTerm)term2, out term2vl);
                    if (term2vl != null && term2vl.IsPred())
                    {
                        Pred pvl = (Pred)term2vl.Clone();
                        pvl.ClearAnnots();
                        Bind((VarTerm)term2, pvl);
                    }
                }
            }
            return ok;
        }

        public bool UnifyTerms(ITerm term, ITerm term2)
        {
            if (term.IsArithExpr())
            {
                term = term.CApply(this);
            }
            if (term2.IsArithExpr())
            {
                term2 = term2.CApply(this);
            }

            bool termIsVar = term.IsVar();
            bool term2IsVar = term2.IsVar();

            // One of them is a variable
            if (termIsVar || term2IsVar)
            {
                VarTerm termv = termIsVar ? (VarTerm)term : null;
                VarTerm term2v = term2IsVar ? (VarTerm)term2 : null;

                // Get their values
                ITerm termvl = termIsVar ? Get(termv) : term;
                ITerm term2vl = term2IsVar ? Get(term2v) : term2;

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

            // Both terms are not vars
            // If any of the terms is not a literal (is a number or string), they must be equal
            // For unification, lists are literals
            if (!term.IsLiteral() && !term.IsList() || !term2.IsLiteral() && !term2.IsList()) return term.Equals(term2);

            // Case of plan body
            if (term.IsPlanBody() && term2.IsPlanBody())
            {
                IPlanBody pb1 = (IPlanBody)term;
                IPlanBody pb2 = (IPlanBody)term2;

                if (pb1.GetBodyTerm() == null && pb2.GetBodyTerm() == null) return true;
                if (pb1.GetBodyTerm() == null && pb2.GetBodyTerm() != null) return false;
                if (pb1.GetBodyTerm() != null && pb2.GetBodyTerm() == null) return false;

                if (pb1.GetBodyTerm().IsVar() && pb2.GetBodyTerm().IsVar())
                {
                    if (UnifiesNoUndo(pb1.GetBodyTerm(), pb2.GetBodyTerm()))
                    {
                        return UnifiesNoUndo(pb1.GetBodyNext(), pb2.GetBodyNext());
                    }
                    else return false;
                }

                if (pb1.GetBodyTerm().IsVar())
                {
                    if (pb1.GetBodyNext() == null) return UnifiesNoUndo(pb1.GetBodyTerm(), pb2);
                    else
                    {
                        if (pb2.GetBodyTerm() == null) return false;
                        if (UnifiesNoUndo(pb1.GetBodyTerm(), pb2.GetHead()))
                        {
                            if (pb2.GetBodyNext() == null)
                            {
                                if (pb1.GetBodyNext() != null && pb1.GetBodyNext().GetBodyTerm().IsVar() && pb1.GetBodyNext().GetBodyNext() == null)
                                {
                                    return UnifiesNoUndo(pb1.GetBodyNext().GetBodyTerm(), new PlanBodyImpl());
                                }
                                return false;
                            }
                            else return UnifiesNoUndo(pb1.GetBodyNext(), pb2.GetBodyNext());
                        }
                    }
                }
                else if (pb2.GetBodyTerm().IsVar()) return Unifies(pb2, pb1);
            }

            // Both terms are literal
            Literal t1s = (Literal)term;
            Literal t2s = (Literal)term2;

            // Different arities
            int ts = t1s.GetArity();
            if (ts != t2s.GetArity()) return false;

            // If both are literal, they must have the same negated
            if (t1s.Negated() != t2s.Negated()) return false;

            // Different functor
            if (!t1s.GetFunctor().Equals(t2s.GetFunctor())) return false;

            // Different namespace
            if (!UnifiesNamespace(t1s, t2s)) return false;

            // Unify inner terms
            for (int i = 0; i < ts; i++)
            {
                if (!UnifiesNoUndo(t1s.GetTerm(i), t2s.GetTerm(i))) return false;
            }

            // The first's annotations must be a subset of the second's annotations
            if (!t1s.HasSubsetAnnot(t2s, this)) return false;

            return true;
        }

        private bool UnifiesNamespace(Literal t1s, Literal t2s)
        {
            // If both are the default NS
            if (t1s == Literal.DefaultNS && t2s == Literal.DefaultNS) return true;

            // Compares namespaces of t1s and t2s
            t1s = t1s.GetNS();
            t2s = t2s.GetNS();
            // Faster than UnifiesNoUndo
            if (t1s.Equals(t2s)) return true;
            return UnifiesNoUndo(t1s, t2s);
        }

        public VarTerm Deref(VarTerm term)
        {
            ITerm vl;
            function.TryGetValue(term, out vl);
            VarTerm first = term;
            while (vl != null && term.IsVar())
            {
                term = (VarTerm)vl;
                function.TryGetValue(term, out vl);
            }
            if (first != term)
            {
                function.Add(first, vl);
            }
            return term;
        }

        public bool Bind(VarTerm vt, ITerm term)
        {
            if (vt.Negated()) // Negated variables unify only with negated literals
            {
                if (!term.IsLiteral() || !((Literal)term).Negated())
                {
                    return false;
                }
                term = (Literal)term.Clone();
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
                    term = (ITerm)lterm.CloneNS(Literal.DefaultNS);
                }
            }
            if (!term.IsCyclicTerm() && term.HasVar(vt, this))
            {
                term = new CyclicTerm((Literal)term, (VarTerm)vt.Clone());
            }
            function.Add(GetVarForUnifier(vt), term);
            return true;
        }

        public VarTerm GetVarForUnifier(VarTerm term)
        {
            term = (VarTerm)Deref(term).CloneNS(Literal.DefaultNS);
            //Esta funcion debe devolver un literal
            term.SetNegated(Literal.LPos);
            return term;
        }

        public void Clear() => function.Clear();

        public override string ToString() => function.ToString();

        public ITerm GetAsTerm()
        {
            IListTerm lf = new ListTermImpl();
            IListTerm tail = lf;
            foreach (VarTerm k in function.Keys)
            {
                ITerm vl;
                function.TryGetValue(k, out vl);
                vl.Clone(); // Como uso el Clone de C# lo que clono son object que luego hay que castear...
                if (vl is Literal) ((Literal)vl).MakeVarsAnnon();
                // Variable must be changed to avoid cyclic references later
                Structure pair = AsSyntax.AsSyntax.CreateStructure("dictionary", UnnamedVar.Create(k.ToString()), vl);
                tail = tail.Append(pair);
            }
            return lf;
        }

        public int Size() => function.Count;

        public void Compose(Unifier u)
        {
            foreach (VarTerm k in u.function.Keys)
            {
                ITerm current = Get(k);
                ITerm kValue;
                u.function.TryGetValue(k, out kValue);
                // Current unifier has the new var
                if (current != null && (current.IsVar() || kValue.IsVar())) Unifies(kValue, current);
                else function.Add((VarTerm)k.Clone(), (ITerm)kValue.Clone()); // Como uso el Clone de C# lo que clono son object que luego hay que castear...
            }
        }

        public Unifier Clone()
        {
            try
            {
                Unifier newUn = new Unifier();
                newUn.function = CloneFunction();
                return newUn;
            }
            catch
            {
                return null;
            }
        }

        public int HashCode()
        {
            int s = 0;
            foreach (VarTerm v in function.Keys) s += v.GetHashCode();
            return s * 31;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o == this) return true;
            if (o is Unifier) return function.Equals(((Unifier)o).function);
            return false;
        }

        public void SetDictionary(Dictionary<VarTerm, ITerm> newFunc) => function = newFunc;

        public Dictionary<VarTerm, ITerm>  GetFunction()
        {
            return function;
        }

        public IEnumerator<VarTerm> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
