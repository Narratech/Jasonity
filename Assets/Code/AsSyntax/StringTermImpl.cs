using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Logic.AsSyntax
{
    public class StringTermImpl : DefaultTerm, StringTerm
    {
        private static readonly long serialVersionUID = 1L;
        private readonly string value; 

        public StringTermImpl():base()
        {
            value = null;
        }

        public StringTermImpl(string fs)
        {
            value = fs;
        }

        public StringTermImpl(StringTermImpl t)
        {
            value = t.GetString();
            srcInfo = t.srcInfo;
        }

        public string GetString()
        {
            return value;
        }

        public new StringTerm Clone()
        {
            return this;
        }

        public override Term Clone()
        {
            throw new NotImplementedException();
        }

        public static StringTerm ParseString(string sTerm)
        {
            as2j parser = new as2j(new StringReader(sTerm));
            try
            {
                return (StringTerm)parser.Term();
            }
            catch (Exception e)
            {
                Debug.Log("Error parsing term"+ sTerm);
                return null;
            }
        }

        public override bool IsString()
        {
            return true;
        }

        public int Lenght()
        {
            if (value == null)
                return 0;
            else
                return value.Length;
        }

        public override bool Equals(object t)
        {
            if (t == this) return true;

            if (t != null && (t.GetType() == typeof(StringTerm))) {
                StringTerm st = (StringTerm)t;
                if (value == null)
                    return st.GetString() == null;
                else
                    return value.Equals(st.GetString());
            }
            return false;
        }

        public override int? CalcHashCode()
        {
            if (value == null)
                return 0;
            else
                return value.GetHashCode();
        }

        public override int CompareTo(Term o)
        {
            if (o.GetType() == typeof(VarTerm))
            {
                return o.CompareTo(this) * -1;
            }
            if(o.GetType() == typeof(NumberTerm))
            {
                return 1;
            }
            return base.CompareTo(o);
        }

        public new string ToString()
        {
            return "\"" + value + "\"";
        }
    }
}
