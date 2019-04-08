using System;
using System.Collections.Generic;

using Assets.Code.Logic.AsSyntax.parser;
using Assets.Code.AsSyntax;

namespace Assets.Code.Logic.parser
{ 
    public partial class as2j : as2jConstants {

        private String asSource = "no-asl-source";
        private Agent.Agent curAg = null;

        private Atom @namespace = Literal.DefaultNS;
        private Atom thisnamespace = Literal.DefaultNS;

        //carpeta directive de asSyntax
        private DirectiveProcessor directiveProcessor = new DirectiveProcessor();
        private NameSpace nsDirective = (NameSpace)directiveProcessor.getInstance("namespace");

        private static HashSet<string> parsedFiles = new HashSet<String>();
        //carpeta util jason
        private static Config config = Config.get(false);
        //No encuentro esta clase
        private static Pattern patternUnnamedWithId = Pattern.compile("_(\\d+)(.*)");

        public void SetAg(Agent.Agent ag) { curAg = ag; }
        public void SetNS(Atom  ns) { @namespace = ns; thisnamespace = ns; }
        public Atom GetNS()         { return @namespace; }

        public void SetASLSource(String src) { asSource = src; }
        
        private String GetSourceRef(SourceInfo s) {
            if (s == null)
                return "[]";
            else
                return "["+s.GetSrcFile()+":"+s.GetBeginSrcLine()+"]";
        }

        private String GetSourceRef(DefaultTerm t) {
            return GetSourceRef( t.GetSrcInfo());
        }

        private String GetSourceRef(Object t) {
            if (t.GetType() == typeof(DefaultTerm)) {
                return GetSourceRef((DefaultTerm)t);
            }
            
            else if (t.GetType() == typeof (SourceInfo)) {
                return GetSourceRef((SourceInfo)t);
            }

            else
                return "[]";
        }

        private InternalActionLiteral CheckInternalActionsInContext(ILogicalFormula f, Agent.Agent ag) {
            if (f != null) {
                if (f.GetType() == typeof(InternalActionLiteral)) {
                    InternalActionLiteral ial = (InternalActionLiteral)f;
                    //InternalAction de carpeta Agent sin hacer
                    if (!ial.GetIA(ag).canBeUsedInContext()) {
                        return ial;
                    }
                } 
                
                else if (f.GetType() == typeof (LogExpr)) {
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

        private ArithFunctionTerm GetArithFunction(Literal l) {
            ArithFunctionTerm af = null;
            if (curAg != null)
               // try to find the function in agent register
               af = curAg.getFunction(l.GetFunctor(), l.GetArity());
            if (af == null)
               // try global function
               //carpeta directiva de asSyntax
               af = FunctionRegister.getFunction(l.GetFunctor(), l.GetArity());
            return af;
        }

        private ITerm ChangeToAtom(Object o) {
            ITerm u = (ITerm)o;
            if (u == Literal.LTrue)
                return u;
            if (u == Literal.LFalse)
                return u;
            if (u.IsAtom()) {
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