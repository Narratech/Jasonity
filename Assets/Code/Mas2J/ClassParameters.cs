using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.AsSyntax;

namespace Assets.Code.Mas2J
{
    public class ClassParameters
    {
        private string className;
        private List<string> parameters = new List<string>();
        private string host;

        public ClassParameters() { }
        public ClassParameters(string className) => this.className = className;
        public ClassParameters(Structure s)
        {
            className = s.GetFunctor();
            if (s.GetArity() > 0)
                foreach (ITerm t in s.GetTerms())
                    parameters.Add(t.ToString());
        }

        public ClassParameters Copy()
        {
            ClassParameters newcp = new ClassParameters(className);
            newcp.parameters = new List<string>(parameters);
            newcp.host = host;
            return newcp;
        }

        public void SetClassName(string cn) => className = cn;
        public string GetClassName() => className;

        public void AddParameter(string s) => parameters.Add(s);
        public ICollection<string> GetParameters() => parameters;
        public string GetParameter(int index)
        {
            if (parameters.Count > index) return parameters[index];
            else return null;
        }
        public string GetParameter(string startWith)
        {
            foreach (string s in parameters)
                if (s.StartsWith(startWith))
                    return s;
            return null;
        }
        public bool HasParameter(string s) => parameters.Contains(s);
        public bool HasParameters() => parameters.Count != 0;
        public string[] GetParametersArray()
        {
            string[] p = new string[parameters.Count];
            int i = 0;
            foreach (string s in parameters)
                p[i++] = RemoveQuotes(s);
            return p;
        }

        public object[] GetTypedParametersArray()
        {
            object[] p = new object[parameters.Count];
            int i = 0;
            foreach (string s in parameters)
            {
                //Esto lo hacía directamaente sobre s en Java
                string sAux;
                sAux = RemoveQuotes(s);
                try
                {
                    p[i] = int.Parse(sAux);
                }
                catch (Exception e)
                {
                    try
                    {
                        p[i] = double.Parse(sAux);
                    }
                    catch (Exception e3)
                    {
                        if (sAux.Equals("true")) p[i] = true;
                        else if (sAux.Equals("false")) p[i] = false;
                        else p[i] = sAux;
                    }
                }
                i++;
            }
            return p;
        }

        // Returns parameters with space as separator
        public string GetParametersStr(string sep)
        {
            StringBuilder sb = new StringBuilder();
            if (parameters.Count > 0)
            {
                IEnumerator<string> i = parameters.GetEnumerator();
                while (i.MoveNext())
                {
                    sb.Append(i.Current);
                    if (i.MoveNext()) sb.Append(sep);
                }
            }
            return sb.ToString();
        }

        public void SetHost(string h)
        {
            if (h.StartsWith("\""))
                host = h.Substring(1, h.Length - 1);
            else
                host = h;
        }
        public string GetHost() => host;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(className);
            if (parameters.Count > 0)
            {
                sb.Append("(");
                IEnumerator<string> e = parameters.GetEnumerator();
                while (e.MoveNext())
                {
                    sb.Append(e.MoveNext());
                    if (e.MoveNext()) sb.Append(",");
                }
                sb.Append(")");
            }
            return sb.ToString();
        }

        string RemoveQuotes(string s)
        {
            if (s.StartsWith("\"") && s.EndsWith("\"")) return s.Substring(1, s.Length - 1);
            else return s;
        }

        public override int GetHashCode() => className.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            if (!(obj is ClassParameters)) return false;
            ClassParameters o = (ClassParameters)obj;
            if (!className.Equals(o.className)) return false;
            return true;
        }
    }
}
