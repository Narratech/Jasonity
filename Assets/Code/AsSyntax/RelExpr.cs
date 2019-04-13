using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelExpr : BinaryStructure, ILogicalFormula
{
    private RelationalOp op = RelationalOp.none;
    public RelExpr(ITerm t1, RelationalOp oper, ITerm t2) : base(t1, oper.ToString(), t2) => op = oper;

    public new IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
    {
        ITerm xp = GetTerm(0).Capply(un);
        ITerm yp = GetTerm(1).Capply(un);

        if (op.GetType() == RelationalOp.none.GetType()) { }

        else if (op.GetType() == RelationalOp.gt.GetType())
        {
            if (xp.CompareTo(yp) > 0) return LogExpr.CreateUnifEnumerator(un);
        }

        else if (op.GetType() == RelationalOp.gte.GetType())
        {
            if (xp.CompareTo(yp) >= 0) return LogExpr.CreateUnifEnumerator(un);
        }
            
        else if (op.GetType() == RelationalOp.lt.GetType())
        {
            if (xp.CompareTo(yp) < 0) return LogExpr.CreateUnifEnumerator(un);
        }

        else if (op.GetType() == RelationalOp.lte.GetType())
        {
            if (xp.CompareTo(yp) <= 0) return LogExpr.CreateUnifEnumerator(un);
        }
                   
        else if (op.GetType() == RelationalOp.eq.GetType())
        {
            if (xp.Equals(yp)) return LogExpr.CreateUnifEnumerator(un);
        }
                    
        else if (op.GetType() == RelationalOp.dif.GetType())
        {
            if (!xp.Equals(yp)) return LogExpr.CreateUnifEnumerator(un);
        }
        else if (op.GetType() == RelationalOp.unify.GetType())
        {
            if (un.Unifies(xp, yp)) return LogExpr.CreateUnifEnumerator(un);
        }

        else if (op.GetType() == RelationalOp.literalBuilder.GetType())
        {
            try
            {
                Literal p = (Literal)xp; // LHS clone
                IListTerm l = (IListTerm)yp; // RHS clone

                // Both are not vars, using normal unfiication
                if (!p.IsVar() && !l.IsVar())
                {
                    IListTerm palt = p.GetAsListOfTerms();
                    if (l.Size() == 3) // list without name space
                        palt = palt.GetNext();
                    if (un.Unifies(palt, l))
                        return LogExpr.CreateUnifEnumerator(un);
                }
                else
                {
                    // First is var, second is list, var is assigned to l transformed in literal
                    if (p.IsVar() && l.IsList())
                    {
                        ITerm t = null;
                        if (l.Size() == 4 && l[3].IsPlanBody())
                            t = Plan.NewFromListOfTerms(l);
                        else
                            t = Literal.NewFromListOfTerms(l);
                        if (un.Unifies(p, t))
                            return LogExpr.CreateUnifEnumerator(un);
                        else
                            LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
                    }

                    // First is literal, second is var, var is assigned to l transformed in list
                    if (p.IsLiteral() && l.IsVar())
                    {
                        if (un.Unifies(p.GetAsListOfTerms(), l))
                            return LogExpr.CreateUnifEnumerator(un);
                        else
                            LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
                    }
                }
            }
            catch (Exception e)
            {
                // Mensaje de error
            }
        }

        return LogExpr.EMPTY_UNIF_LIST.GetEnumerator();
    }

    public override ITerm Capply(Unifier u) => new RelExpr(GetTerm(0).Capply(u), op, GetTerm(1).Capply(u));

    // Make a hard copy of the terms
    public new ILogicalFormula Clone() => new RelExpr(GetTerm(0).Clone(), op, GetTerm(1).Clone());

    public override Literal CloneNS(Atom newnamespace) => new RelExpr(GetTerm(0).CloneNS(newnamespace), op, GetTerm(1).CloneNS(newnamespace));

    public RelationalOp GetOp() => op;

    public class RelationalOp
    {
        public readonly static RelationalOp none;
        public readonly static RelationalOp gt;
        public readonly static RelationalOp gte;
        public readonly static RelationalOp lt;
        public readonly static RelationalOp lte;
        public readonly static RelationalOp eq;
        public readonly static RelationalOp dif;
        public readonly static RelationalOp unify;
        public readonly static RelationalOp literalBuilder;

        public override string ToString()
        {
            if (this == none) return "";
            else if (this == gt) return " > ";
            else if (this == gte) return " >= ";
            else if (this == lt) return " < ";
            else if (this == lte) return " <= ";
            else if (this == eq) return " == ";
            else if (this == dif) return " \\== ";
            else if (this == unify) return " = ";
            else if (this == literalBuilder) return " =.. ";
            else return null;
        }
    }
}
