using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
   Represents a logical formula with some logical operator ("&amp;",  "|", "not").

   @navassoc - op - LogicalOp

 */
namespace Jason.Logic.AsSyntax
{
    public class LogExpr : BinaryStructure, LogicalFormula
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Term other)
        {
            throw new NotImplementedException();
        }
    }
}
