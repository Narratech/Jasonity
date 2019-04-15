using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Assets.Code.ReasoningCycle;
using System.Collections;
using Assets.Code.Exceptions;
using Assets.Code.BDIAgent;

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
namespace Assets.Code.AsSyntax
{

    public abstract class Literal : DefaultTerm, ILogicalFormula
    {

        public static readonly bool LPos = true;
        public static readonly bool LNeg = false;

        public static readonly Atom DefaultNS = new DefaultNameSpace();
        public static readonly Literal LTrue = new TrueLiteral();
        public static readonly Literal LFalse = new FalseLiteral();

        // to not compute it all the time (it is used many many times)
        protected PredicateIndicator predicateIndicatorCache = null;

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
        public virtual PredicateIndicator GetPredicateIndicator()
        {
            if (predicateIndicatorCache == null)
            {
                predicateIndicatorCache = new PredicateIndicator(GetNS(), GetFunctor(), GetArity());
            }
            return predicateIndicatorCache;
        }

        /* default implementation of some methods */

        /** returns the number of terms of this literal */
        public virtual int GetArity()
        {
            return 0;
        }

        /** returns true if this literal has some term */
        public virtual bool HasTerm()
        {
            return false;
        }

        public virtual List<ITerm> GetTerms()
        {
            return Structure.emptyTermList;
        }

        /** returns all terms of this literal */
        public ITerm[] GetTermsArray()
        {
            if (HasTerm())
            {
                return GetTerms().ToArray(Structure.emptyTermArray);
                //Structure.emptyTermArray
            }
            else
            {
                return Structure.emptyTermArray;
            }
        }

        private static readonly List<VarTerm> emptyListVar = new List<VarTerm>();
        /** returns all singleton vars (that appears once) in this literal */
        public List<VarTerm> GetSingletonVars()
        {
            return emptyListVar;
        }

        /** replaces all terms by unnamed variables (_). */
        public virtual void MakeTermsAnnon() { }
        /** replaces all variables by unnamed variables (_). */
        public virtual Literal MakeVarsAnnon()
        {
            return this;
        }

        /**
         * replaces all variables of the term for unnamed variables (_).
         *
         * @param un is the unifier that contains the map of replacements
         */
        public virtual Literal MakeVarsAnnon(Unifier un)
        {
            return this;
        }

        /** returns all annotations of the literal */
        public virtual IListTerm GetAnnots()
        {
            return null;
        }
        /** returns true if there is some annotation <i>t</i> in the literal */
        public virtual bool HasAnnot(ITerm t)
        {
            return false;
        }

        /** returns true if the pred has at least one annot */
        public virtual bool HasAnnot()
        {
            return false;
        }

        /** returns true if all this predicate annots are in p's annots */
        public virtual bool HasSubsetAnnot(Literal p)
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
        public virtual bool HasSubsetAnnot(Literal p, Unifier u)
        {
            return true;
        }

        /** removes all annotations */
        public virtual void ClearAnnots() { }

        /**
         * returns all annots with the specified functor e.g.: from annots
         * [t(a), t(b), source(tom)]
         * and functor "t",
         * it returns [t(a),t(b)]
         *
         * in case that there is no such an annot, it returns an empty list.
         */
        public virtual IListTerm GetAnnots(string functor)
        {
            return new ListTermImpl();
        }

        /** returns the first annotation (literal) that has the <i>functor</i> */
        public virtual Literal GetAnnot(string functor)
        {
            return null;
        }

        /**
         * returns the sources of this literal as a new list. e.g.: from annots
         * [source(a), source(b)], it returns [a,b]
         */
        public virtual IListTerm GetSources()
        {
            return new ListTermImpl();
        }

        /** returns true if this literal has some source annotation */
        public virtual bool HasSource()
        {
            return false;
        }

        /** returns true if this literal has a "source(<i>agName</i>)" */
        public virtual bool HasSource(ITerm agName)
        {
            return false;
        }

        /** returns this if this literal can be added in the belief base (Atoms, for instance, can not be) */
        public virtual bool CanBeAddedInBB()
        {
            return false;
        }

        /** returns this if this literal should be removed from BB while doing BUF */
        public bool SubjectToBUF()
        {
            return true;
        }

        /** returns whether this literal is negated or not, use Literal.LNeg and Literal.LPos to compare the returned value */
        public virtual bool Negated()
        {
            return false;
        }

        public virtual bool EqualsAsStructure(object p)
        {
            return false;
        }

        // structure
        public virtual void AddTerm(ITerm t)
        {
        }

        public virtual void DelTerm(int index)
        {
        }

        /** adds some terms and return this */
        public virtual Literal AddTerms(params ITerm[] ts)
        {
            return null;
        }

        /** adds some terms and return this */
        public virtual Literal AddTerms(List<ITerm> l)
        {
            return null;
        }

        /** returns the i-th term (first term is 0) */
        public virtual ITerm GetTerm(int i)
        {
            return null;
        }

        /** set all terms of the literal and return this */
        public virtual Literal SetTerms(List<ITerm> l)
        {
            return null;
        }

        public virtual void SetTerm(int i, ITerm t)
        {
        }

        // pred
        public virtual Literal SetAnnots(IListTerm l)
        {
            return null;
        }

        public virtual bool AddAnnot(ITerm t)
        {
            return false;
        }

        /** adds some annots and return this */
        public virtual Literal AddAnnots(params ITerm[] ts)
        {
            return null;
        }

        /** adds some annots and return this */
        public virtual Literal AddAnnots(List<ITerm> l)
        {
            return null;
        }

        public virtual bool DelAnnot(ITerm t)
        {
            return false;
        }

        /**
         * removes all annots in this pred that are in the list <i>l</i>.
         * @return true if some annot was removed.
         */
        public virtual bool DelAnnots(List<ITerm> l)
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
        public virtual bool ImportAnnots(Literal p)
        {
            return false;
        }

        /** adds the annotation source(<i>agName</i>) */
        public virtual void AddSource(ITerm agName)
        {
        }

        /** deletes one source(<i>agName</i>) annotation, return true if deleted */
        public virtual bool DelSource(ITerm agName)
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

        public virtual IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
        {
            IEnumerator<Literal> il = ag.GetBB().GetCandidateBeliefs(this, un);
            if (il == null)
            {
                return LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
            }

            AgentArchitecture arch = (ag != null && ag.GetReasoner() != null ? ag.GetReasoner().GetUserAgArch() : null);
            int nbAnnots = (HasAnnot() && GetAnnots().GetTail() == null ? GetAnnots().Count : 0);

            return new MyIEnumerator<Unifier>(arch, nbAnnots, il, ag, un);
            
        }

        private class MyIEnumerator<Unifier>: IEnumerator<Unifier>
        {
            Unifier current = default;
            IEnumerator<Unifier> ruleIt = null; // current rule solutions iterator
            Literal cloneAnnon = null; // a copy of the literal with makeVarsAnnon
            Rule rule; // current rule
            bool needsUpdate = true;
            Agent ag;
            Unifier un;
            IEnumerator<List<ITerm>> annotsOptions = null;
            Literal belInBB = null;
            private AgentArchitecture arch;
            private int nbAnnots;
            private IEnumerator<Literal> il;

            public Unifier Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public MyIEnumerator(AgentArchitecture arch, int nbAnnots, IEnumerator<Literal> il, Agent ag, Unifier un)
            {
                this.arch = arch;
                this.nbAnnots = nbAnnots;
                this.il = il;
                this.ag = ag;
                this.un = un;
            }

            public bool HasNext()
            {
                if (needsUpdate)
                    Get();
                return current != null;
            }

            public Unifier Next()
            {
                if (needsUpdate)
                    Get();
                if (current != null)
                    needsUpdate = true;
                return current;
            }

            private void Get()
            {
                needsUpdate = false;
                current = default;
                if (arch != null && !arch.IsRunning()) return;

                if (annotsOptions != null)
                {
                    while (annotsOptions.MoveNext())
                    {
                        Literal belToTry = belInBB.Copy().SetAnnots(null).AddAnnots(annotsOptions.Current);
                        Unifier u = un.Clone();
                        if (u.UnifiesNoUndo(this, belToTry))
                        {
                            current = u;
                            return;
                        }
                    }
                    annotsOptions = null;
                }

                if (ruleIt != null)
                {
                    while (ruleIt.MoveNext())
                    {
                        Unifier ruleUn = ruleIt.Current;
                        Literal rHead = rule.HeadCApply(ruleUn);
                        UseDerefVars(rHead, ruleUn);
                        rHead.MakeVarsAnnon();

                        Unifier unC = un.Clone();
                        if (unC.UnifiesNoUndo(this, rHead))
                        {
                            current = unC;
                            return;
                        }
                    }
                    ruleIt = null;
                }

                while (il.MoveNext())
                {
                    belInBB = il.Current;
                    if (belInBB.IsRule())
                    {
                        rule = (Rule)belInBB;
                        if (cloneAnnon == null)
                        {
                            cloneAnnon = (Literal)this.Capply(un);
                            cloneAnnon.MakeVarsAnnon();
                        }

                        Unifier ruleUn = new Unifier();
                        if (ruleUn.UnifiesNoUndo(cloneAnnon, rule))
                        {
                            ruleIt = rule.GetBody().LogicalConsequence(ag, ruleUn);
                            Get();
                            if (current != null)
                                return;
                        }
                        else
                        {
                            if (nbAnnots > 0)
                            {
                                if (belInBB.HasAnnot())
                                {
                                    int nbAnnotsB = belInBB.GetAnnots().Count;
                                    if (nbAnnotsB >= nbAnnots)
                                    {
                                        annotsOptions = belInBB.GetAnnots().SubSets(nbAnnots);
                                        Get();
                                        if (current != null)
                                            return;
                                    }
                                }
                            }
                            else
                            {
                                Unifier u = un.Clone();
                                if (u.UnifiesNoUndo(this, belInBB))
                                {
                                    current = u;
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            public void Remove() { }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        private void UseDerefVars(ITerm p, Unifier un)
        {
            if (p.GetType() == typeof(Literal))
            {
                Literal l = p as Literal;
                for (int i = 0; i < l.GetArity(); i++)
                {
                    ITerm t = l.GetTerm(i);
                    if (t.IsVar())
                    {
                        l.SetTerm(i, un.Deref(t as VarTerm));
                    }
                    else
                    {
                        UseDerefVars(t, un);
                    }
                }
            }
        }

        /** returns this literal as a list with three elements: [functor, list of terms, list of annots] */
        public IListTerm GetAsListOfTerms()
        {
            IListTerm l = new ListTermImpl();
            l.Add(GetNS());
            l.Add(new LiteralImpl(!Negated(), GetFunctor()));
            IListTerm lt = new ListTermImpl();
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

        /** creates a literal from a list with four elements: [namespace, functor, list of terms, list of annots]
         *  (namespace is optional)
         */
        public static Literal NewFromListOfTerms(IListTerm lt)
        {
            try
            {
                System.Collections.Generic.IEnumerator<ITerm> i = lt.GetEnumerator();

                Atom ns = DefaultNS;
                if (lt.Count == 4)
                {
                    ns = i.Current as Atom;
                }
                ITerm tFunctor = i.Current;

                bool pos = Literal.LPos;
                if (tFunctor.IsLiteral() && (tFunctor as Literal).Negated())
                {
                    pos = Literal.LNeg;
                }
                if (tFunctor.IsString())
                {
                    tFunctor = AsSyntax.ParseTerm((tFunctor as IStringTerm).GetString());
                }

                Literal l = new LiteralImpl(ns, pos, (tFunctor as Atom).GetFunctor());

                if (i.Current != null)
                {
                    //l.SetTerms((i.Current as IListTerm).CloneLT());
                    return null;
                }
                if (i.Current != null)
                {
                    l.SetAnnots((i.Current as IListTerm).CloneLT());
                }
                return l;
            }
            catch (Exception)
            {
                throw new JasonityException("Error creating literal from"+lt);
            }
        }

        /**
         * Transforms this into a full literal (which implements all methods of Literal), if it is an Atom;
         * otherwise returns 'this'
         */
        public Literal ForceFullLiteralImpl()
        {
            if (this.IsAtom() && !(this.GetType() == typeof(LiteralImpl)))
            {
                return new LiteralImpl(this);
            }
            else
            {
                return this;
            }
        }

        sealed class TrueLiteral : Atom
        {
            public TrueLiteral() : base("true")
            {

            }

            public override Literal CloneNS(Atom newNamespace)
            {
                return this;
            }

            public override ITerm Capply(Unifier u)
            {
                return this;
            }

            public override IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
            {
                return LogExpr.CreateUnifEnumerator(un);
            }

            object ReadResolve()
            {
                return LTrue;
            }
        }

        sealed class FalseLiteral: Atom
        {
            public FalseLiteral() : base("false")
            {

            }

            public override Literal CloneNS(Atom newNamespace)
            {
                return this;
            }

            public override ITerm Capply(Unifier u)
            {
                return this;
            }

            public override IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
            {
                return LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
            }

            object ReadResolve()
            {
                return LFalse;
            }
        }

        private sealed class DefaultNameSpace : Atom
        {
            public DefaultNameSpace() : base(null, "default")
            {

            }

            new int CalcHashCode()
            {
                return GetFunctor().GetHashCode();
            }

            public override ITerm Capply(Unifier u)
            {
                return this;
            }

            public override Literal CloneNS(Atom newnamespace)
            {
                return this;
            }

            public override Atom GetNS()
            {
                return this;
            }

            public override bool Equals(object o)
            {
                if (o == null) return false;
                if (o == this) return true;
                if (o.GetType() == typeof(Atom))
                {
                    Atom a = o as Atom;
                    return a.IsAtom() && GetFunctor().Equals(a.GetFunctor());
                }
                return false;
            }

            public new string ToString()
            {
                return GetFunctor();
            }

            object ReadResolver()
            {
                return DefaultNS;
            }
        }

        public override ITerm Clone()
        {
            throw new NotImplementedException();
        }

        public override int CalcHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
