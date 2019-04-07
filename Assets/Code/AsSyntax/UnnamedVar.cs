using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Assets.Code.AsSyntax
{

    /*
     * Represents the unnamed variable '_'
     */
    public class UnnamedVar:VarTerm
    {
        private static readonly long serialVersionUID = 1L;

        private readonly AtomicInteger varCont = new AtomicInteger(0);
        public int myId;

        public UnnamedVar():base(Literal.DefaultNS, varCont.IncrementAndGet())
        {

        }

        public UnnamedVar(Atom ns):base(ns, varCont.incrementandget())
        {

        }

        public UnnamedVar(Atom ns, int id):base(ns, "_" + id)
        {
            myId = id;
        }

        public UnnamedVar(int id):base("_" + id)
        {
            myId = id;
        }

        // do not allow the creation of unnamed var by name since the myId attribute should be defined!
        // this constructor is for internal use (see create below)
        private UnnamedVar(string name):base(name)
        {
        }

        private UnnamedVar(Atom ns, string name):base(ns, name)
        {
        }

        public static UnnamedVar Create(string name)
        {
            return Create(Literal.DefaultNS, name);
        }

        public static UnnamedVar Create(Atom ns, string name)
        {
            if (name.Length == 1)
            {
                return new UnnamedVar(ns);
            }
            else
            {
                int id = varCont.incrementAndGet();
                UnnamedVar v = new UnnamedVar(ns, "_" + id + name);
                v.myId = id;
                return v;
            }
        }

        public static UnnamedVar Create(Atom ns, int id, string name)
        {
            UnnamedVar v = new UnnamedVar(ns, name);
            v.myId = id * -1;
            return v;
        }

        public new DefaultTerm Clone()
        {
            return CloneNS(GetNS());
        }

        public override Literal CloneNS(Atom newNamespace)
        {
            UnnamedVar newv = new UnnamedVar(newNamespace, GetFunctor());
            newv.myId = this.myId;
            newv.hashCodeCache = this.hashCodeCache;
            if (HasAnnot())
                newv.AddAnnots(this.GetAnnots().CloneLT());
            return newv;
        }

        public override bool Equals(object t)
        {
            if (t == null) return false;
            if (t == this) return true;
            if (t.GetType() == typeof(UnnamedVar))
                return ((UnnamedVar)t).myId == this.myId;
            return false;
        }

        public new int CompareTo(ITerm t)
        {
            if (t.GetType() == typeof(UnnamedVar))
            {
                if (myId > ((UnnamedVar)t).myId)
                    return 1;
                else if (myId < ((UnnamedVar)t).myId)
                    return -1;
                else return 0;
            }
            else if (t.GetType() == typeof(VarTerm))
            {
                return 1;
            }
            else
            {
                return base.CompareTo(t);
            }
        }

        public override bool IsUnnamedVar()
        {
            return true;
        }
    }
}
