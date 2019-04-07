using Assets.Code.Agent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Code.AsSyntax
{
    public class LogExpr : BinaryStructure, ILogicalFormula
    {
        public static List<Unifier> EMPTY_UNIF_LIST = new List<Unifier>();
        private LogicalOp op = LogicalOp.none;

        public LogExpr(ILogicalFormula f1, LogicalOp oper, ILogicalFormula f2) : base(f1, oper.ToString(), f2) => op = oper;

        public LogExpr(LogicalOp oper, ILogicalFormula f) : base(oper.ToString(), f) => op = oper;

        public ILogicalFormula GetLHS() => (ILogicalFormula)GetTerm(0);

        public ILogicalFormula GetRHS() => (ILogicalFormula)GetTerm(1);

        public IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
        {
            /* try
            {
                switch (op)
                {
                    case LogicalOp.none:
                        break;
                    case LogicalOp.not:
                        if (!GetLHS().LogicalConsequence(ag, un).HasNext())
                        {
                            return CreateUnifEnumerator(un);
                        }
                        break;
                    case LogicalOp.and:
                        return new IEnumerator<Unifier>()
                        {
                            IEnumerator <Unifier> ileft = GetLHS().LogicalConsequence(ag, un);
                            IEnumerator<Unifier> iright = null;
                            Unifier current = null;
                            bool needsUpdate = true;

                            public bool HasNext()
                            {
                                if (needsUpdate) Get();
                                return current != null;
                            }
                        }
                }
            }
            */
        }

       // public IEnumerator<Unifier> CreateUnifEnumerator(Unifier... unifs) { }

        public ITerm Capply(Unifier u)
        {
            if (IsUnary()) return new LogExpr(op, (ILogicalFormula)GetTerm(0).Capply(u));
            else return new LogExpr((ILogicalFormula)GetTerm(0).Capply(u), op, (ILogicalFormula)GetTerm(1).Capply(u));
        }

        public ILogicalFormula Clone()
        {
            if (IsUnary()) return new LogExpr(op, (ILogicalFormula)GetTerm(0).Clone());
            else return new LogExpr((ILogicalFormula)GetTerm(0).Clone(), op, (ILogicalFormula)GetTerm(1).Clone());
        }

        public Literal CloneNS(Atom newnamespace)
        {
            if (IsUnary()) return new LogExpr(op, (ILogicalFormula)GetTerm(0).CloneNS(newnamespace));
            else return new LogExpr((ILogicalFormula)GetTerm(0).CloneNS(newnamespace), op, (ILogicalFormula)GetTerm(1).CloneNS(newnamespace));
        }

        public LogicalOp GetOp() => op;

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class LogicalOp
    {
        public readonly static LogicalOp none;
        public readonly static LogicalOp not;
        public readonly static LogicalOp and;
        public readonly static LogicalOp or;

        public override string ToString()
        {
            if (this == none) return "";
            else if (this == not) return "not ";
            else if (this == and) return " & ";
            else if (this == or) return " | ";
            else return null;
        }
    }
}