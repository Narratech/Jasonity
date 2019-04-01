using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.Logic.AsSyntax;

namespace Assets.Code.Logic
{
    public class Literal : DefaultTerm, LogicalFormula
    {
        public override bool IsLiteral()
        {
            return true;
        }

        public override Term Clone()
        {
            throw new NotImplementedException();
        }

        protected override int? CalcHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
