using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logica.ASSemantic;
using UnityEngine;
using System.IO;

/**
 This class represents an abstract literal (an Atom, Structure, Predicate, etc), it is mainly
 the interface of a literal.

 To create a new Literal, one of the following concrete classes may be used:
 <ul>
 <li> Atom -- the most simple literal, is composed by only a functor (no term, no annots);
 <li> Structure -- has functor and terms;
 <li> Pred -- has functor, terms, and annotations;
 <li> LiteralImpl -- Pred + negation.
 </ul>
 The latter class supports all the operations of
 the Literal interface.

 <p>There are useful static methods in class {@link ASSyntax} to create Literals.

 @see ASSyntax
 @see Atom
 @see Structure
 @see Pred
 @see LiteralImpl

 */
namespace Jason.Logic.AsSyntax
{
    public abstract class Literal : DefaultTerm, LogicalFormula
    {
        private static readonly long serialVersionUID = 1L;

        public static readonly bool LPos   = true;
        public static readonly bool LNeg   = false;

        public static readonly Atom    DefaultNS = new DefaultNameSpace();
        public static readonly Literal LTrue     = new TrueLiteral();
        public static readonly Literal LFalse    = new FalseLiteral();

        protected PredicateIndicator predicateIndicatorCache = null; // to not compute it all the time (it is used many many times)

        /** creates a new literal by parsing a string -- ASSyntax.parseLiteral or createLiteral are preferred. */
        public static Literal ParseLiteral(string sLiteral)
        {
            try
            {
                as2j parser = new as2j(new StringReader(sLiteral));
                return parser.literal();
            }
            catch (Exception)
            {
                Debug.Log("Error parsing literal " + sLiteral);
                return null;
            }
        }

        public Literal Copy()
        {
            return (Literal)Clone(); // should call the clone, that is overridden in subclasses
        }

        /** returns the functor of this literal */
        public abstract string GetFunctor();

        /** returns the name spaceof this literal */
        public abstract Atom GetNS();

        public bool IsLiteral()
        {
            return true;
        }

        /** returns name space :: functor symbol / arity */
        public PredicateIndicator GetPredicateIndicator()
        {
            if (predicateIndicatorCache == null)
            {
                predicateIndicatorCache = new PredicateIndicator(GetNS(), GetFunctor(), GetArity());
            }
            return predicateIndicatorCache;
        }

        /* default implementation of some methods */

        /** returns the number of terms of this literal */
        public int GetArity()
        {
            return 0;
        }
        /** returns true if this literal has some term */
        public bool HasTerm()
        {
            return false;
        }
        /** returns all terms of this literal */
        public List<Term> GetTerms()
        {
            return Structure.emptyTermList;
        }

        /** returns all terms of this literal as an array */
        public Term[] GetTermsArray()
        {
            if (HasTerm())
                return GetTerms().ToArray(Structure.emptyTermArray);
            else
                return Structure.emptyTermArray;
        }

        private static readonly List<VarTerm> emptyListVar = new List<VarTerm>();
        /** returns all singleton vars (that appears once) in this literal */
        public List<VarTerm> GetSingletonVars()
        {
            return emptyListVar;
        }

        /** replaces all terms by unnamed variables (_). */
        public void MakeTermsAnnon() { }
        /** replaces all variables by unnamed variables (_). */
        public Literal MakeVarsAnnon()
        {
            return this;
        }

        /**
         * replaces all variables of the term for unnamed variables (_).
         *
         * @param un is the unifier that contains the map of replacements
         */
        public Literal MakeVarsAnnon(Unifier un)
        {
            return this;
        }

        /** returns all annotations of the literal */
        public ListTerm GetAnnots()
        {
            return null;
        }
        /** returns true if there is some annotation <i>t</i> in the literal */
        public bool HasAnnot(Term t)
        {
            return false;
        }

        /** returns true if the pred has at least one annot */
        public bool HasAnnot()
        {
            return false;
        }

        /** returns true if all this predicate annots are in p's annots */
        public bool HasSubsetAnnot(Literal p)
        {
            return true;
        }

        /**
         * Returns true if all this predicate's annots are in p's annots using the
         * unifier u.
         *
         * if p annots has a Tail, p annots's Tail will receive this predicate's annots,
         * e.g.:
         *   this[a,b,c] = p[x,y,b|T]
         * unifies and T is [a,c] (this will be a subset if p has a and c in its annots).
         *
         * if this annots has a tail, the Tail will receive all necessary term
         * to be a subset, e.g.:
         *   this[b|T] = p[x,y,b]
         * unifies and T is [x,y] (this will be a subset if T is [x,y].
         */
        public bool HasSubsetAnnot(Literal p, Unifier u)
        {
            return true;
        }

        /** removes all annotations */
        public void ClearAnnots() { }

        /**
         * returns all annots with the specified functor e.g.: from annots
         * [t(a), t(b), source(tom)]
         * and functor "t",
         * it returns [t(a),t(b)]
         *
         * in case that there is no such an annot, it returns an empty list.
         */
        public ListTerm GetAnnots(string functor)
        {
            return new ListTermImpl();
        }

        /** returns the first annotation (literal) that has the <i>functor</i> */
        public Literal GetAnnot(string functor)
        {
            return null;
        }

        /**
         * returns the sources of this literal as a new list. e.g.: from annots
         * [source(a), source(b)], it returns [a,b]
         */
        public ListTerm GetSources()
        {
            return new ListTermImpl();
        }
        /** returns true if this literal has some source annotation */
        public bool HasSource()
        {
            return false;
        }
        /** returns true if this literal has a "source(<i>agName</i>)" */
        public bool HasSource(Term agName)
        {
            return false;
        }

        /** returns this if this literal can be added in the belief base (Atoms, for instance, can not be) */
        public bool CanBeAddedInBB()
        {
            return false;
        }
        /** returns this if this literal should be removed from BB while doing BUF */
        public bool SubjectToBUF()
        {
            return true;
        }

        /** returns whether this literal is negated or not, use Literal.LNeg and Literal.LPos to compare the returned value */
        public bool Negated()
        {
            return false;
        }

        public bool EqualsAsStructure(object p)
        {
            return false;
        }

        /* Not implemented methods */
        // structure
        public void AddTerm(Term t)
        {
            Debug.Log("Is not implemented in this class");
        }

        public void DelTerm(int index)
        {
            Debug.Log("Is not implemented in this class");
        }

        public Literal AddTerms(Term /* ... */ ts)
        {
            Debug.Log("Is not implemented in this class");
            return null;
        }

        public Literal AddTerms(List<Term> l)
        {
            Debug.Log("Is not implemented in this class");
            return null;
        }

        public Term GetTerm(int i)
        {
            Debug.Log("Is not implemented in this class");
            return null;
        }

        public Literal SetTerms(List<Term> l)
        {
            Debug.Log("Is not implemented in this class");
            return null;
        }

        public void SetTerm(int i, Term t)
        {
            Debug.Log("Is not implemented in this class");
        }

        public Literal SetAnnots(ListTerm l)
        {
            Debug.Log("Is not implemented in this class");
            return null;
        }

        public bool AddAnnot(Term t)
        {
            Debug.Log("Is not implemented in this class");
            return false;
        }

        public Literal AddAnnots(Term terms)
        {
            Debug.Log("Is not implemented in this class");
            return null;
        }

        public Literal AddAnnots(List<Term> l)
        {
            Debug.Log("Is not implemented in this class");
            return null;
        }

        public bool DelAnnot(Term t)
        {
            Debug.Log("Is not implemented in this class");
            return false;
        }

        public bool DelAnnots(List<Term> t)
        {
            Debug.Log("Is not implemented in this class");
            return false;
        }

        public bool ImportAnnots(Literal p)
        {
            Debug.Log("Is not implemented in this class");
            return false;
        }

        public void AddSource(Term agName)
        {
            Debug.Log("Is not implemented in this class");
        }

        public bool DelSource(Term agName)
        {
            Debug.Log("Is not implemented in this class");
            return false;
        }

        public void DelSources()
        {
            Debug.Log("Is not implemented in this class");
        }

        public Literal SetNegated(bool b)
        {
            Debug.Log("Is not implemented in this class"); ;
            return null;
        }

        /**
         * logicalConsequence checks whether one particular predicate
         * is a logical consequence of the belief base.
         *
         * Returns an iterator for all unifiers that are logCons.
         */
        public IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
        {
            IEnumerator<Literal> il   = ag.getBB().getCandidateBeliefs(this, un);
            if (il == null) // no relevant bels
                return LogExpr.EMPTY_UNIF_LIST.iterator();

            AgArch arch = (ag != null && ag.getTS() != null ? ag.getTS().getUserAgArch() : null);
            int nbAnnots = (HasAnnot() && GetAnnots().GetTail() == null ? GetAnnots().size() : 0); // if annots contains a tail (as in p[A|R]), do not backtrack on annots

            //ERROR INFUMABLE
            IEnumerator<Unifier>() { };

        }

        public void UseDerefVars(Term p, Unifier uni)
        {
            if (p.GetType() == typeof(Literal))
            {
                Literal l = (Literal)p;
                for (int i = 0; i < l.GetArity(); i++)
                {
                    Term t = l.GetTerm(i);
                    if (t.IsVar())
                    {
                        l.SetTerm(i, t);
                    }
                    else
                    {
                        UseDerefVars(t, uni);
                    }
                }
            }

        }

        /** returns this literal as a list with three elements: [functor, list of terms, list of annots] */
        public ListTerm getAsListOfTerms()
        {
            ListTerm l = new ListTermImpl();
            l.Add(GetNS());
            l.Add(new LiteralImpl(!Negated(), GetFunctor()));
            ListTerm lt = new ListTermImpl();
            lt.addAll(GetTerms());
            l.Add(lt);
            if (HasAnnot())
            {
                l.Add(GetAnnots().CloneLT());
            }
            else
            {
                l.Add(new ListTermImpl());
            }
            return l;
        }

        public static Literal newFromListOfTerms(ListTerm lt)
        {
            try
            {
                IEnumerator<Term> i = lt.iterator();

                Atom ns = DefaultNS;
                if (lt.size() == 4)
                    ns = (Atom)i.MoveNext();

                Term tfunctor = i.MoveNext();

                bool pos = Literal.LPos;
                if (tfunctor.IsLiteral() && ((Literal)tfunctor).Negated())
                {
                    pos = Literal.LNeg;
                }
                if (tfunctor.IsString())
                {
                    tfunctor = ASSyntax.parseTerm(((StringTerm)tfunctor).getString());
                }

                Literal l = new LiteralImpl(ns, pos, ((Atom)tfunctor).GetFunctor());

                if (i.hasNext())
                {
                    l.setTerms(((ListTerm)i.next()).CloneLT());
                }
                if (i.hasNext())
                {
                    l.setAnnots(((ListTerm)i.next()).CloneLT());
                }
                return l;
            }
            catch (Exception e)
            {
                throw new JasonException("Error creating literal from " + lt);
            }
        }

        /**
         * Transforms this into a full literal (which implements all methods of Literal), if it is an Atom;
         * otherwise returns 'this'
         */
        public Literal forceFullLiteralImpl()
        {
            if (this.IsAtom() && !(this.GetType() == typeof(LiteralImpl)))
                return new LiteralImpl(this);
            else
                return this;
        }

        class TrueLiteral: Atom
        {
            public TrueLiteral()
            {
                base("true");
            }

            
            Literal cloneNS(Atom newnamespace)
            {
                return this;
            }

            Term capply(Unifier u)
            {
                return this;
            }

            IEnumerator<Unifier> logicalConsequence(Agent ag, Unifier un)
            {
                return LogExpr.createUnifIterator(un);
            }

            protected object readResolve()
            {
                return Literal.LTrue;
            }
        }

        class FalseLiteral: Atom
        {
            public FalseLiteral()
            {
                base("false");
            }

            public Literal cloneNS(Atom newnamespace)
            {
                return this;
            }

            public Term capply(Unifier u)
            {
                return this;
            }

            public IEnumerator<Unifier> logicalConsequence(Agent ag, Unifier un)
            {
                return LogExpr.EMPTY_UNIF_LIST.iterator();
            }

            protected object readResolve()
            {
                return Literal.LFalse;
            }
        }

        private class DefaultNameSpace: Atom
        {
            public DefaultNameSpace()
            {
                base(null, "default");
            }

            protected int calcHashCode()
            {
                return GetFunctor().hashCode();
            }

            public Term capply(Unifier u)
            {
                return this;
            }

            public Literal cloneNS(Atom newnamespace)
            {
                return this;
            }

            public Atom getNS()
            {
                return this;
            }

            public bool equals(object o)
            {
                if (o == null) return false;
                if (o == this) return true;
                if (o.GetType() == typeof(Atom)) {
                    Atom a = (Atom)o;
                    return a.IsAtom() && GetFunctor().Equals(a.GetFunctor());
                }
                return false;
            }

            public string ToString()
            {
                return GetFunctor();
            }

            protected object readResolve()
            {
                return Literal.DefaultNS;
            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Term other)
        {
            throw new NotImplementedException();
        }
    }
}
