using Assets.Code.AsSyntax;
using Assets.Code.Logic.parser;
using System.Collections.Generic;
using System.Threading;

namespace Assets.Code.Logic.AsSyntax.directives
{ 
    public partial class NameSpace : IDirective {

        public const string LOCAL_PREFIX = "#";

        private Dictionary<Atom, Atom> localNSs = new Dictionary<Atom, Atom>();

        public bool IsSingleton()
        {
            return false;
        }

        public Agent.Agent Process(Pred directive, Agent.Agent outerContent, Agent.Agent innerContent)
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
            //CAMBIAR: Funcion get similar en diccionarios de c#
            return localNSs[ns] != null;
        }

        public Atom Map(Atom ns)
        {
            Atom n = localNSs[ns];
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