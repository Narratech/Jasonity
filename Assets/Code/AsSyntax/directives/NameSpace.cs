using Assets.Code.BDIAgent;
using Assets.Code.parser;
using System.Collections.Generic;
using System.Threading;

namespace Assets.Code.AsSyntax.directives
{ 
    public class NameSpace : IDirective {

        public const string LOCAL_PREFIX = "#";

        private Dictionary<Atom, Atom> localNSs = new Dictionary<Atom, Atom>();

        public bool IsSingleton()
        {
            return false;
        }

        public Agent Process(Pred directive, Agent outerContent, Agent innerContent)
        {
            return innerContent;
        }

        Stack<Atom> oldNS = new Stack<Atom>();

        public void Begin(Pred directive, as2j parser)
        {
            if (!directive.GetTerm(0).IsAtom())
            {
                return;
            }
            Atom ns = new Atom(((Atom)directive.GetTerm(0)).GetFunctor());

            if (directive.GetArity() > 1)
            {
                if (!directive.GetTerm(1).IsAtom())
                {
                    
                    return;
                }
                string type = ((Atom)directive.GetTerm(1)).GetFunctor();
                if (!type.Equals("local") && !type.Equals("global"))
                {
                    return;
                }
                if (type.Equals("global") && IsLocalNS(ns))
                {
                    localNSs.Remove(ns);
                }
                if (type.Equals("local"))
                {
                    ns = AddLocalNS(ns);
                }
            }

            oldNS.Push(parser.GetNS());
            parser.SetNS(ns);
        }

        public void End(Pred directive, as2j parser)
        {
            if (oldNS.Peek() != null) 
                parser.SetNS(oldNS.Pop());
        }

        public bool IsLocalNS(Atom ns)
        {
            return localNSs[ns] != null;
        }

        public Atom Map(Atom ns)
        {
            Atom n = null; 
            localNSs.TryGetValue(ns, out n);
            if (n == null)
                return ns;
            else
                return n;
        }

        static private int nsCounter = 0;

        public static int GetUniqueID()
        {
            return Interlocked.Increment(ref nsCounter);
        }

        private Atom AddLocalNS(Atom ns)
        {
            Atom newNS = localNSs[ns];
            if (newNS == null)
            {
                newNS = new Atom(LOCAL_PREFIX + Interlocked.Increment(ref nsCounter) + ns);
                localNSs[ns] = newNS;
            }
            return newNS;
        }
    }
}