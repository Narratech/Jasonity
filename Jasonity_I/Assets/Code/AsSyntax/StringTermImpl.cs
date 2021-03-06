﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.AsSyntax
{
    public class StringTermImpl : DefaultTerm, IStringTerm
    {
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

        //public IStringTerm CloneAux()
        //{
        //    return this;
        //}

        // En vez de ITerm Clone() voy a poner object Clone()
        public override object Clone()
        {
            return this; // Uy, que peligroso es no clonar sino devolver lo mismo. Hacer aunque sea un wisemember clone...
        }

        public override bool IsString()
        {
            return true;
        }

        public int Length()
        {
            if (value == null)
                return 0;
            else
                return value.Length;
        }

        public override bool Equals(object t)
        {
            if (t == this) return true;

            if (t != null && (t.GetType() == typeof(IStringTerm))) {
                IStringTerm st = (IStringTerm)t;
                if (value == null)
                    return st.GetString() == null;
                else
                    return value.Equals(st.GetString());
            }
            return false;
        }

        public override int CalcHashCode()
        {
            if (value == null)
                return 0;
            else
                return value.GetHashCode();
        }

        public override int CompareTo(ITerm o)
        {
            if (o.GetType() == typeof(VarTerm))
            {
                return o.CompareTo(this) * -1;
            }
            if(o.GetType() == typeof(INumberTerm))
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
