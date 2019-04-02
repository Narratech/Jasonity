using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Assets.Code.Logic.AsSyntax;

/*
 *  This class represents an abstract literal (an Atom, Structure, Predicate, etc), it is mainly
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
 */
namespace Assets.Code.Logic
{

    public abstract class Literal : DefaultTerm, LogicalFormula
    {
        private static readonly long serialVersionUID = 1L;

        public static readonly bool LPos = true;
        public static readonly bool LNeg = false;

        public static readonly Atom DefaultNS = new DefaultNameSpace();
        public static readonly Literal LTrue = new TrueLiteral();
        public static readonly Literal LFalse = new FalseLiteral();

        // to not compute it all the time (it is used many many times)
        protected PredicateIndicator predicateIndicatorCache = null;

        //Creates a new literal by parsing a string
        public static Literal ParseLiteral(string sLiteral)
        {
            try
            {
                as2j parser = new as2j(new StringReader(sLiteral));
                return parser.Literal();
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public Literal Copy()
        {
            // should call the clone, that is overridden in subclasses
            return Clone() as Literal;
        }

        /** returns the functor of this literal */
        public abstract string GetFunctor();

        /** returns the name spaceof this literal */
        public abstract Atom GetNS();

        public override bool IsLiteral()
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
        public Term[] GetTermsArray()
        {
            if (HasTerm())
            {
                return GetTerms().ToArray(Structure.EmptyTermArray);
            }
            else
            {
                return Structure.EmptyTermArray;
            }
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
        }

        public void DelTerm(int index)
        {
        }

        /** adds some terms and return this */
        public Literal AddTerms(params Term[] ts)
        {
            return null;
        }

        /** adds some terms and return this */
        public Literal AddTerms(List<Term> l)
        {
            return null;
        }

        /** returns the i-th term (first term is 0) */
        public Term GetTerm(int i)
        {
            return null;
        }

        /** set all terms of the literal and return this */
        public Literal SetTerms(List<Term> l)
        {
            return null;
        }

        public void SetTerm(int i, Term t)
        {
        }

        // pred
        public Literal SetAnnots(ListTerm l)
        {
            return null;
        }

        public bool AddAnnot(Term t)
        {
            return false;
        }

        /** adds some annots and return this */
        public Literal AddAnnots(params Term[] ts)
        {
            return null;
        }

        /** adds some annots and return this */
        public Literal AddAnnots(List<Term> l)
        {
            return null;
        }

        public bool DelAnnot(Term t)
        {
            return false;
        }

        /**
         * removes all annots in this pred that are in the list <i>l</i>.
         * @return true if some annot was removed.
         */
        public bool DelAnnots(List<Term> l)
        {
            return false;
        }

        /**
         * "import" annots from another predicate <i>p</i>. p will be changed
         * to contain only the annots actually imported (for Event),
         * for example:
         *     p    = b[a,b]
         *     this = b[b,c]
         *     after import, p = b[a]
         * It is used to generate event <+b[a]>.
         *
         * @return true if some annot was imported.
         */
        public bool ImportAnnots(Literal p)
        {
            return false;
        }

        /** adds the annotation source(<i>agName</i>) */
        public void AddSource(Term agName)
        {
        }

        /** deletes one source(<i>agName</i>) annotation, return true if deleted */
        public bool DelSource(Term agName)
        {
            return false;
        }

        /** deletes all source annotations */
        public void DelSources()
        {
        }

        // literal
        /** changes the negation of the literal and return this */
        public Literal SetNegated(bool b)
        {
            return null;
        }

        public IEnumerator<Unifier> LogicalConsecuence(Agent ag, Unifier un)
        {

        }







        public override Term Clone()
        {
            throw new NotImplementedException();
        }

        protected override int? CalcHashCode()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
        {
            throw new NotImplementedException();
        }
    }
}
