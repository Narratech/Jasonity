using Assets.Code.BDIAgent;
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

        public new ILogicalFormula GetLHS() => (ILogicalFormula)GetTerm(0);

        public new ILogicalFormula GetRHS() => (ILogicalFormula)GetTerm(1);

        public new IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un)
        {
            try
            {
                if (op.GetType() == LogicalOp.none.GetType()) return null;
                else if (op.GetType() == LogicalOp.not.GetType())
                    if (!GetLHS().LogicalConsequence(ag, un).MoveNext()) return CreateUnifEnumerator(un);
                else if (op.GetType() == LogicalOp.and.GetType())
                    return new AndIterator<Unifier>(this, ag, un);
                else if (op.GetType() == LogicalOp.or.GetType())
                    return new OrIterator<Unifier>(this, ag, un);
            }
            catch (Exception e)
            {
                string slhs = "is null ";
                IEnumerator<Unifier> i = GetLHS().LogicalConsequence(ag, un);
                if (i != null)
                {
                    slhs = "";
                    while (i.MoveNext())
                        slhs += i.MoveNext().ToString() + ", ";
                }
                else
                    slhs = "iterator is null";
                string srhs = "is null ";
                if (!IsUnary())
                {
                    i = GetRHS().LogicalConsequence(ag, un);
                    if (i != null)
                    {
                        srhs = "";
                        while (i.MoveNext())
                            srhs += i.MoveNext().ToString() + ", ";
                    }
                    else
                        srhs = "iterator is null";
                }
            }
            return EMPTY_UNIF_LIST.GetEnumerator();
        }

        public static IEnumerator<Unifier> CreateUnifEnumerator(params Unifier[] unifs)
        {
            return new MyUnifEnumerator<Unifier>(unifs);
        }

        private class MyUnifEnumerator<Unifier> : IEnumerator<Unifier>
        {
            public Unifier Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            Unifier[] unifs;

            int i;

            public MyUnifEnumerator(params Unifier[] unifs)
            {
                i = 0;
                this.unifs = unifs;
            }

            public bool HasNext()
            {
                return i < unifs.Length;
            }

            public Unifier Next()
            {
                return unifs[i++];
            }

            public void Remove() { }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        public override ITerm Capply(Unifier u)
        {
            if (IsUnary()) return new LogExpr(op, (ILogicalFormula)GetTerm(0).Capply(u));
            else return new LogExpr((ILogicalFormula)GetTerm(0).Capply(u), op, (ILogicalFormula)GetTerm(1).Capply(u));
        }

        public new ILogicalFormula Clone()
        {
            if (IsUnary()) return new LogExpr(op, (ILogicalFormula)GetTerm(0).Clone());
            else return new LogExpr((ILogicalFormula)GetTerm(0).Clone(), op, (ILogicalFormula)GetTerm(1).Clone());
        }

        public new Literal CloneNS(Atom newnamespace)
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

    public class AndIterator<T> : IEnumerator<T> where T : Unifier
    {
        private LogExpr logExpr;
        private Agent ag;
        private Unifier un;
        AndIterator<Unifier> ileft;
        AndIterator<Unifier> iright = null;
        Unifier current = default;
        bool needsUpdate = true;

        public AndIterator(LogExpr logExpr, Agent ag, Unifier un)
        {
            this.logExpr = logExpr;
            this.ag = ag;
            this.un = un;
            ileft = (AndIterator<Unifier>)logExpr.GetLHS().LogicalConsequence(ag, un);
        }

        public bool HasNext()
        {
            if (needsUpdate) Get();
            return current != null;
        }

        public Unifier Next()
        {
            if (needsUpdate) Get();
            if (current != null) needsUpdate = true;
            return current;
        }

        private void Get()
        {
            needsUpdate = false;
            current = default;
            while ((iright == null || iright.MoveNext()) && ileft.MoveNext())
                iright = (AndIterator<Unifier>)logExpr.GetRHS().LogicalConsequence(ag, ileft.Current);
            if (iright != null && iright.HasNext())
                current = iright.Next();
        }

        public void Remove() { }

        public Unifier Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        T IEnumerator<T>.Current => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class OrIterator<T> : IEnumerator<Unifier>
    {
        private LogExpr logExpr;
        private Agent ag;
        private Unifier un;
        OrIterator<Unifier> ileft;
        OrIterator<Unifier> iright;
        Unifier current = default;
        bool needsUpdate = true;

        public OrIterator(LogExpr logExpr, Agent ag, Unifier un)
        {
            this.logExpr = logExpr;
            this.ag = ag;
            this.un = un;
            ileft = (OrIterator<Unifier>)logExpr.GetLHS().LogicalConsequence(ag, un);
        }

        public bool HasNext()
        {
            if (needsUpdate) Get();
            return current != null;
        }

        public Unifier Next()
        {
            if (needsUpdate) Get();
            if (current != default) needsUpdate = true;
            return current;
        }

        private void Get()
        {
            needsUpdate = false;
            current = default;
            if (ileft != null && ileft.HasNext())
                current = ileft.Next();
            else
            {
                if (iright == null)
                    iright = (OrIterator<Unifier>)logExpr.GetRHS().LogicalConsequence(ag, un);
                if (iright != null && iright.HasNext())
                    current = iright.Next();
            }
        }

        public void Remove() { }

        public Unifier Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}