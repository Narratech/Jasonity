using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    class Literal
    {
        internal static Atom DefaultNS;
        internal static object LPos;

        internal bool HasSubsetAnnot(Literal bl)
        {
            throw new NotImplementedException();
        }

        internal bool HasAnnot(object percept)
        {
            throw new NotImplementedException();
        }

        internal bool DelAnnots(object p)
        {
            throw new NotImplementedException();
        }

        internal object GetAnnots()
        {
            throw new NotImplementedException();
        }

        internal bool HasSource()
        {
            throw new NotImplementedException();
        }

        internal bool IsRule()
        {
            throw new NotImplementedException();
        }

        internal bool IsVar()
        {
            throw new NotImplementedException();
        }

        internal bool ImportAnnots(Literal l)
        {
            throw new NotImplementedException();
        }

        internal Literal Copy()
        {
            throw new NotImplementedException();
        }

        internal Atom GetNS()
        {
            throw new NotImplementedException();
        }

        internal PredicateIndicator GetPredicateIndicator()
        {
            throw new NotImplementedException();
        }

        public static explicit operator Literal(Term v)
        {
            throw new NotImplementedException();
        }

        public static explicit operator Literal(void v)
        {
            throw new NotImplementedException();
        }

        public static explicit operator Literal(Pred v)
        {
            throw new NotImplementedException();
        }
    }
}
