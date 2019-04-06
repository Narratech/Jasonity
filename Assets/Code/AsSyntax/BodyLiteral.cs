using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic.AsSyntax
{
    public class BodyLiteral:PlanBodyImpl
    {
        public BodyLiteral(BodyType t, Term b)
        {
            base(OldToNew(t), b);
        }

        private static PlanBody.BodyType OldToNew(BodyType old)
        {
            switch (old)
            {
                case action:
                    return PlanBody.BodyType.action;
                case internalAction:
                    return PlanBody.BodyType.internalAction;
                case achieve:
                    return PlanBody.BodyType.achieve;
                case test:
                    return PlanBody.BodyType.test;
                case addBel:
                    return PlanBody.BodyType.addBel;
                case delBel:
                    return PlanBody.BodyType.delBel;
                case delAddBel:
                    return PlanBody.BodyType.delAddBel;
                case achieveNF:
                    return PlanBody.BodyType.achieveNF;
                case constraint:
                    return PlanBody.BodyType.constraint;
                default:
                    break;
            }
        }
    }
}
