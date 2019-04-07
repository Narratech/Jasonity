using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    public class Pred : Structure
    {
        private IListTerm annotations;

        public Pred(string functor) : base(functor)
        {

        }

        public Pred(Literal l):this(l.GetNS(), l)
        {

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

        public Pred(Literal l, Unifier u) : base(l, u)
        {
            if (l.HasAnnot())
            {
                SetAnnots((IListTerm)l.GetAnnots().Capply(u));
            } else
            {
                annotations = null;
            }
        }

        public Pred(string functor, int termSize) : base(functor, termSize)
        {

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

        private override Literal SetAnnots(IListTerm listTerm)
        {
            annotations = null;
            if (listTerm == null)
            {
                return this;
            }
            IEnumerator<IListTerm> en = listTerm.ListTermIterator();
            while(en.MoveNext())
            {
                IListTerm lt = en.Current;
                if(lt.GetTerm() == null)
                {
                    return this;
                }
                AddAnnot(lt.GetTerm());
                if(lt.IsTail())
                {
                    annotations.SetTail(lt.GetTail());
                    return this;
                }
            }
            return this;
        }

        public override bool AddAnnot(ITerm t)
        {
            if (annotations == null)
            {
                annotations = new ListTermImpl();
            }
            IEnumerator<IListTerm> en = annotations.ListTermIterator();
            while (en.MoveNext())
            {
                IListTerm lt = en.Current;
                int c = t.CompareTo(lt.GetTerm());
                if (c == 0)
                { 
                    return false;
                }
                else if (c < 0)
                {
                    lt.Insert(t);
                    return true;
                }
            }
            return false;
        }

        public override Literal AddAnnots(List<ITerm> l)
        {
            if (l != null)
            {
                foreach (ITerm t in l)
                {
                    AddAnnot(t);
                }
            }
            return this;
        }

        public override Literal AddAnnots(params ITerm[] l)
        {
            foreach (ITerm t in l)
            {
                AddAnnot(t);
            }
            return this;
        }
        
        public override bool DelAnnot(ITerm t)
        {
            if (annotations == null)
            {
                return false;
            } else
            {
                return annotations.Remove(t); //I dont undestand this
            }
        }

        public override Literal ClearAnnots()
        {
            annotations = null;
            return this;
        }

        public override IListTerm GetAnnots()
        {
            return annotations;
        }

        public override bool HasAnnot(ITerm t)
        {
            if (annotations == null)
            {
                return false;
            }
            IEnumerator<IListTerm> en = annotations.ListTermIterator();
            while (en.MoveNext())
            {
                IListTerm lt = en.Current;
                int c = t.CompareTo(lt.GetTerm());
                if (c == 0)
                { 
                    return true;
                }
                else if (c < 0)
                {
                    return false;
                }
            }
            return false;
        }

        public override Literal GetAnnot(string functor)
        {
            if (annotations == null)
            {
                return null;
            }
            foreach (ITerm t in annotations) //okay (?)
            {
                if (t.IsLiteral())
                {
                    Literal l = (Literal)t;
                    int c = functor.CompareTo(l.GetFunctor());
                    if (c == 0)
                    { 
                        return l;
                    }
                    else if (c < 0)
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        
        public override bool HasAnnot()
        {
            return annotations != null && !annotations.IsEmpty(); //??
        }

        public override bool HasVar(VarTerm t, Unifier u)
        {
            if (base.HasVar(t, u))
            {
                return true;
            }
            if (annotations != null)
            {
                foreach (ITerm v in annotations)
                {
                    if (v.HasVar(t, u))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void CountVars(Dictionary<VarTerm, int?> c)
        {
            base.CountVars(c);
            if (annotations != null)
            {
                foreach (ITerm t in annotations)
                {
                    t.CountVars(c);
                }
            }
        }

        public override bool ImportAnnots(Literal p)
        {
            bool imported = false;
            if (p.HasAnnot())
            {
                IEnumerator<ITerm> en = p.GetAnnots().ListTermIterator();
                while (en.MoveNext())
                {
                    ITerm t = en.Current;
                    if (AddAnnot(t.Clone()))
                    {
                        imported = true;
                    }
                    else
                    {
                        en.Remove(); //I dont know this
                    }
                }
            }
            return imported;
        }

        public override bool DelAnnots(List<ITerm> l)
        {
            bool removed = false;
            if (l != null && HasAnnot())
            {
                foreach (ITerm t in l)
                {
                    bool r = DelAnnot(t);
                    removed = removed || r;
                }
            }
            return removed;
        }

        public override IListTerm GetAnnots(string functor)
        {
            IListTerm ls = new ListTermImpl();
            if (annotations != null)
            {
                IListTerm tail = ls;
                foreach (ITerm ta in annotations)
                {
                    if (ta.IsLiteral())
                    {
                        if (((Literal)ta).GetFunctor().Equals(functor))
                        {
                            tail = tail.Append(ta);
                        }
                    }
                }
            }
            return ls;
        }

        public override bool HasSubsetAnnot(Literal p)
        {
            if (annotations == null)
            {
                return true;
            }
            if (HasAnnot() && !p.HasAnnot())
            {
                return false;
            }
            IEnumerator<ITerm> en = p.GetAnnots().ListTermIterator();
            int c = -1;
            foreach (ITerm myAnnot in annotations)
            { 
                if (!en.MoveNext())
                {
                    return false;
                }
                while (en.MoveNext())
                {
                    ITerm t = en.Current;
                    c = myAnnot.CompareTo(t);
                    if (c <= 0)
                    {
                        break;
                    }
                }
                if (c != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool HasSubsetAnnot(Literal p, Unifier u)
        {
            if (annotations == null)
            {
                return true;
            }
            if (!p.HasAnnot())
            {
                return false;
            }
            ITerm thisTail = null;
            IListTerm pAnnots = p.GetAnnots().CloneLTShallow();
            VarTerm pTail = pAnnots.GetTail();
            ITerm pAnnot = null;
            IListTerm pAnnotsTail = null;
            IEnumerator<ITerm> en = pAnnots.ListTermIterator();
            bool enReset = false;

            IEnumerator<IListTerm> en2 = annotations.ListTermIterator(); // use this iterator to get the tail of the list
            while (en2.MoveNext())
            {
                IListTerm lt = en2.Current;
                ITerm annot = lt.GetTerm();
                if (annot == null)
                {
                    break;
                } 
                if (lt.IsTail())
                {
                    thisTail = lt.GetTail();
                }        
                if (annotations.IsVar() && !enReset)
                { 
                    enReset = true;
                    en = pAnnots.ListTermIterator();
                    pAnnot = null;
                }
                bool ok = false;
                while (true)
                {
                    if (pAnnot != null && u.UnifiesNoUndo(annotations, pAnnot))
                    {
                        ok = true;
                        en.Remove();
                        pAnnot = en.Current;
                        break;
                    }
                    else if (pAnnot != null && pAnnot.CompareTo(annot) > 0)
                    {
                        break; 
                    }
                    else if (en.MoveNext())
                    {
                        pAnnot = en.Current;
                    }
                    else
                    {
                        break;
                    }
                } 
                if (!ok && pTail != null)
                {
                    if (pAnnotsTail == null)
                    {
                        pAnnotsTail = (IListTerm)u.Get(pTail);
                        if (pAnnotsTail == null)
                        {
                            pAnnotsTail = new ListTermImpl();
                            u.Unifies(pTail, pAnnotsTail);
                            pAnnotsTail = (IListTerm)u.Get(pTail);
                        }
                    }
                    pAnnotsTail.Add(annot.Clone());
                    ok = true;
                }
                if (!ok)
                {
                    return false;
                } 
            }
            if (thisTail != null)
            {
                u.Unifies(thisTail, pAnnots);
            }
            return true;
        }

        public override Literal AddSource(ITerm agName)
        {
            if (agName != null)
            {
                AddAnnot(CreateSource(agName));
            }
            return this;
        }

        public override bool DelSource(ITerm agName)
        {
            if (annotations != null)
            {
                return DelAnnot(CreateSource(agName));
            }
            else
            {
                return false;
            }
        }

        public static Pred CreateSource(ITerm source)
        {
            Pred s;
            if (source.IsGround())
            {
                s = new PredImpl("source", 1);
            }
            else
            { 
                 s = new Pred("source", 1);
            }
            s.AddTerm(source);
            return s;
        }

        public override IListTerm GetSources()
        {
            IListTerm ls = new ListTermImpl();
            if (annotations != null)
            {
                IListTerm tail = ls;
                foreach (ITerm ta in annotations)
                {
                    if (ta.IsStructure())
                    {
                        Structure tas = (Structure)ta;
                        if (tas.GetFunctor().Equals("source"))
                        {
                            tail = tail.Append(tas.GetTerm(0));
                        }
                    }
                }
            }
            return ls;
        }

        public void DelSources()
        {
            if (annotations != null)
            {
                IEnumerator<ITerm> en = annotations.ListTermIterator();
                while (en.MoveNext())
                {
                    ITerm t = en.Current;
                    if (t.IsStructure())
                    {
                        if (((Structure)t).GetFunctor().Equals("source"))
                        {
                            en.Remove();
                        }
                    }
                }
            }
        }

        public override bool HasSource()
        {
            if (annotations != null)
            {
                foreach (ITerm ta in annotations)
                {
                    if (ta.IsStructure())
                    {
                        if (((Structure)ta).GetFunctor().Equals("source"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override bool HasSource(ITerm agName)
        {
            if (annotations != null)
            {
                return HasAnnot(CreateSource(agName));
            }
            return false;
        } 

        public override Literal MakeVarsAnnon(Unifier un)
        {
            if (annotations != null)
            {
                IListTerm lt = annotations;
                while (!lt.IsEmpty())
                {
                    ITerm ta = lt.GetTerm();
                    if (ta.IsVar())
                    {
                        lt.SetTerm(VarToReplace(ta, un));
                    } else if (ta.GetType() == typeof(Structure))
                    {
                        ((Structure)ta).MakeVarsAnnon(un);
                    }                            
                    if (lt.IsTail() && lt.GetNext().IsVar())
                    {
                        lt.SetNext(VarToReplace(lt.GetNext(), un));
                        break;
                    }
                    lt = lt.GetNext();
                }
            }
            return base.MakeVarsAnnon(un);
        }

        public override bool Equals(Object o)
        {
            if (o == null)
            {
                return false;
            }
            if (o == this)
            {
                return true;
            }
            if (o.GetType() == typeof(Pred))
            {
                Pred p = (Pred)o;
                return base.Equals(o) && this.HasSubsetAnnot(p) && p.HasSubsetAnnot(this);
            } else if (o.GetType() == typeof(Atom) && !HasAnnot()) 
            {
                return base.equals(o);
            }
            return false;
        }

        public override bool EqualsAsStructure(Object p)
        { 
            return base.Equals((ITerm)p);
        }

        public override int CompareTo(ITerm t)
        {
            int c = base.CompareTo(t);
            if (c != 0)
            {
                return c;
            } 
            if (t.IsPred())
            {
                Pred tAsPred = (Pred)t;
                if (GetAnnots() == null && tAsPred.GetAnnots() == null)
                {
                    return 0;
                }
                if (GetAnnots() == null)
                {
                    return -1;
                }
                if (tAsPred.GetAnnots() == null)
                {
                    return 1;
                } 
                IEnumerator<ITerm> pai = tAsPred.GetAnnots().ListTermIterator();
                foreach (ITerm a in GetAnnots())
                {
                    c = a.CompareTo(pai.Current);
                    if (c != 0)
                    {
                        return c;
                    }
                }
                int ats = GetAnnots().Size();
                int ots = tAsPred.GetAnnots().Size();
                if (ats < ots)
                {
                    return -1;
                }
                if (ats > ots)
                {
                    return 1;
                }
            }
            return 0;
        } 
    
        public override ITerm Capply(Unifier u)
        {
            return new Pred(this, u);
        }

        public override ITerm Clone()
        {
            return new Pred(this);
        }

        public override Literal CloneNS(Atom newnamespace)
        {
            return new Pred(newnamespace, this);
        }

        public string ToStringAsTerm()
        {
            return base.ToString();
        }
    }

    class PredImpl : Pred
    {
        public PredImpl(string functor, int termSize) : base(functor, termSize)
        {
        }

        public override ITerm Clone()
        {
            return this;
        }
        
        public override ITerm Capply(Unifier u)
        {
            return this;
        }

        public override bool IsGround()
        {
            return true;
        }

        public override Literal MakeVarsAnnon()
        {
            return this;
        }

        public override Literal MakeVarsAnnon(Unifier un)
        {
            return this;
        }
    }
}