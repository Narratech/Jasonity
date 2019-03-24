using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic.AsSyntax
{
    public interface Term:ICloneable, IComparable
    {
        bool IsVar();
        bool IsUnnamedVar();
        bool IsLiteral();
        bool IsRule();
        bool IsList();
        bool IsString();
        bool IsInternalAction();
        bool IsArithExpr();
        bool IsNumeric();
        bool IsPredicate();
        bool IsGround();
        bool IsStructure();
        bool IsAtom();
        bool IsPlanBody();
        bool IsCyclicTerm();
    }
}
