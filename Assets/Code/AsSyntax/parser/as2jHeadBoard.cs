using Assets.Code.Logic.AsSyntax;
using Assets.Code.Agent;
using Assets.Code.BDIManager;
using Assets.Code.Logic.AsSyntax.parser;

namespace Assets.Code.Logic.parser
{ 
    public partial class as2j : as2jConstants {

        private String    asSource = "no-asl-source";
        private Agent     curAg    = null;

        private Atom      namespace     = Literal.DefaultNS;
        private Atom      thisnamespace = Literal.DefaultNS;

        private DirectiveProcessor directiveProcessor = new DirectiveProcessor();
        private NameSpace nsDirective = (NameSpace)directiveProcessor.getInstance("namespace");

        private static Logger logger = Logger.getLogger("aslparser");
        private static Set<String> parsedFiles = new HashSet<String>();
        private static Config config = Config.get(false);
        private static Pattern patternUnnamedWithId = Pattern.compile("_(\\d+)(.*)");

        public void setAg(Agent ag) { curAg = ag; }
        public void setNS(Atom  ns) { namespace = ns; thisnamespace = ns; }
        public Atom getNS()         { return namespace; }

        public void setASLSource(String src) { asSource = src; }
        
        private String getSourceRef(SourceInfo s) {
            if (s == null)
                return "[]";
            else
                return "["+s.getSrcFile()+":"+s.getBeginSrcLine()+"]";
        }

        private String getSourceRef(DefaultTerm t) {
            return getSourceRef( t.getSrcInfo());
        }

        private String getSourceRef(Object t) {
            if (t instanceof DefaultTerm)
                return getSourceRef((DefaultTerm)t);
            else if (t instanceof SourceInfo)
                return getSourceRef((SourceInfo)t);
            else
                return "[]";
        }

        private InternalActionLiteral checkInternalActionsInContext(LogicalFormula f, Agent ag) {
            if (f != null) {
                if (f instanceof InternalActionLiteral) {
                    InternalActionLiteral ial = (InternalActionLiteral)f;
                    if (! ial.getIA(ag).canBeUsedInContext())
                       return ial;
                } else if (f instanceof LogExpr) {
                    LogExpr le = (LogExpr)f;
                    InternalActionLiteral ial = checkInternalActionsInContext(le.getLHS(), ag);
                    if (ial != null)
                        return ial;
                    if (!le.isUnary())
                        return checkInternalActionsInContext(le.getRHS(), ag);
                }
            }
            return null;
        }

        private ArithFunction getArithFunction(Literal l) {
            ArithFunction af = null;
            if (curAg != null)
               // try to find the function in agent register
               af = curAg.getFunction(l.getFunctor(), l.getArity());
            if (af == null)
               // try global function
               af = FunctionRegister.getFunction(l.getFunctor(), l.getArity());
            return af;
        }

        private Term changeToAtom(Object o) {
            Term u = (Term)o;
            if (u == Literal.LTrue)
                return u;
            if (u == Literal.LFalse)
                return u;
            if (u.isAtom()) {
               if (((Atom)u).getFunctor().equals("default"))
                  return Literal.DefaultNS;
               else if (((Atom)u).getFunctor().equals("this_ns"))
                  return thisnamespace;
               else
                  return new Atom((Literal)u);
            }
            return u;
        }
    }
}