//Esto es la clase parcial

namespace Assets.Code.parser
{

    using Assets.Code.AsSemantics;
    using Assets.Code.AsSyntax;
    using Assets.Code.BDIAgent;
    using Assets.Code.AsSyntax.directives;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    public partial class as2j : as2jConstants
    {
        private string asSource = "no-asl-source";
        private Agent curAg = null;

        private Atom @namespace = Literal.DefaultNS;
        private Atom thisnamespace = Literal.DefaultNS;

        private DirectiveProcessor dProcesor = new DirectiveProcessor();
        private NameSpace nsDirective;

        private static HashSet<string> parsedFiles = new HashSet<string>();
        private static Regex patternUnnamedWithId = new Regex("_(\\d+)(.*)");

        public void SetAg(Agent ag) { curAg = ag; }
        public void SetNS(Atom ns) { @namespace = ns; thisnamespace = ns; }
        public Atom GetNS() { return @namespace; }

        public void SetASLSource(string src) { asSource = src; }

        private string GetSourceRef(SourceInfo s)
        {
            if (s == null)
                return "[]";
            else
                return "[" + s.GetSrcFile() + ":" + s.GetBeginSrcLine() + "]";
        }

        private string GetSourceRef(DefaultTerm t)
        {
            return GetSourceRef(t.GetSrcInfo());
        }
        private string GetSourceRef(object t)
        {
            if (t.GetType() == typeof(DefaultTerm))
                return GetSourceRef((DefaultTerm)t);
            else if (t.GetType() == typeof(SourceInfo))
                return GetSourceRef((SourceInfo)t);
            else
                return "[]";
        }

        private InternalActionLiteral CheckInternalActionsInContext(ILogicalFormula f, Agent ag)
        {
            if (f != null)
            {
                if (f.GetType() == typeof(InternalActionLiteral)) {
                    InternalActionLiteral ial = (InternalActionLiteral)f;
                    if (!ial.GetIA(ag).CanBeUsedInContext())
                        return ial;
                }
                else if (f.GetType() == typeof(LogExpr)) {
                    LogExpr le = (LogExpr)f;
                    InternalActionLiteral ial = CheckInternalActionsInContext(le.GetLHS(), ag);
                    if (ial != null)
                        return ial;
                    if (!le.IsUnary())
                        return CheckInternalActionsInContext(le.GetRHS(), ag);
                }
            }
            return null;
        }

        private ArithFunction GetArithFunction(Literal l)
        {
            ArithFunction af = null;
            if (curAg != null)
                // try to find the function in agent register
                af = curAg.GetFunction(l.GetFunctor(), l.GetArity());
            if (af == null)
                // try global function
                af = FunctionRegister.GetFunction(l.GetFunctor(), l.GetArity());
            return af;
        }

        private ITerm ChangeToAtom(object o)
        {
            ITerm u = (ITerm)o;
            if (u == Literal.LTrue)
                return u;
            if (u == Literal.LFalse)
                return u;
            if (u.IsAtom())
            {
                if (((Atom)u).GetFunctor().Equals("default"))
                    return Literal.DefaultNS;
                else if (((Atom)u).GetFunctor().Equals("this_ns"))
                    return thisnamespace;
                else
                    return new Atom((Literal)u);
            }
            return u;
        }

    }
}