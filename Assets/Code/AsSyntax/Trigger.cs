using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    class Trigger : Structure
    {
        private TEOperator op = TEOperator.add;
        private TEType type = TEType.belief;
        private Literal l;

        public Trigger(TEOperator op, TEType t, Literal l) : base("te", 0)
        {
            this.l = l;
            this.op = op;
            this.type = t;
            SetTrigOp(op);
            SetSrcInfo(l.GetSrcInfo());
        }
    }

    public class TEType
    {
        public readonly static TEType belief;
        public readonly static TEType achieve;
        public readonly static TEType test;

        public override string ToString()
        {
            if (this == belief)
            {
                return "";
            } else if (this == achieve)
            {
                return "!";
            } else if (this == test)
            {
                return "?";
            } else
            {
                return null;
            }
        }
    }

    public class TEOperator
    {
        public readonly static TEOperator add;
        public readonly static TEOperator del;
        public readonly static TEOperator goalState;

        public override string ToString()
        {
            if (this == add)
            {
                return "+";
            }
            else if (this == del)
            {
                return "-";
            }
            else if (this == goalState)
            {
                return "^";
            }
            else
            {
                return null;
            }
        }
    }
}
