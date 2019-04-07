using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    public class BodyLiteral:PlanBodyImpl
    {
        public BodyLiteral(BodyType t, ITerm b): base(OldToNew(t), b)
        {
            
        }

        private static IPlanBody.BodyType OldToNew(BodyType old)
        {
            switch (old)
            {
                case action:
                    return IPlanBody.BodyType.action;
                case internalAction:
                    return IPlanBody.BodyType.internalAction;
                case achieve:
                    return IPlanBody.BodyType.achieve;
                case test:
                    return IPlanBody.BodyType.test;
                case addBel:
                    return IPlanBody.BodyType.addBel;
                case delBel:
                    return IPlanBody.BodyType.delBel;
                case delAddBel:
                    return IPlanBody.BodyType.delAddBel;
                case achieveNF:
                    return IPlanBody.BodyType.achieveNF;
                case constraint:
                    return IPlanBody.BodyType.constraint;
                default:
                    break;
            }
        }
    }
}
