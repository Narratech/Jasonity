using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    public interface IStringTerm:ITerm
    {
        /** gets the Java string represented by this term, it
        normally does not return the same string as toString
        (which enclose the string by quotes)  */
        string GetString();
        int Lenght();
    }
}
