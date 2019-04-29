using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Assets.Code.AsSyntax
{
    public class ObjectTermImpl : DefaultTerm, IObjectTerm
    { 
        private readonly object o;
        private MethodInfo mClone;
        private bool hasTestedClone = false;

        public ObjectTermImpl(object o)
        {
            this.o = o;
        }

        public object GetObject()
        {
            return o;
        }

        public override int CalcHashCode()
        {
            return 0.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (this.o == null) return false;
            if (o == null) return false;

            if (o.GetType() == typeof(ObjectTermImpl))
            {
                return this.o.Equals((o as ObjectTermImpl).o);
            }
            return false;
        }

        public override ITerm Clone()
        {
            try
            {
                if (!hasTestedClone)
                {
                    hasTestedClone = true;
                    mClone = o.GetType().GetMethod("Clone", /*(Type[])*/null);
                }
                if (mClone != null)
                {
                    return new ObjectTermImpl(mClone.Invoke(o, /*(object[])*/null));
                }
            }
            catch(Exception e)
            {

            }
            return this;
        }

        public override string ToString()
        {
            return o.ToString();
        }
    }
}
