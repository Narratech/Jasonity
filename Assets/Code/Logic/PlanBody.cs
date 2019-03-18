using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.Logic;
using UnityEngine;

class PlanBody
{

    public enum BodyType { none, action, internalAction, achieve, test, addBel, addBelNewFocus, addBelBegin, addBelEnd, delBel, delBelNewFocus, delAddBel, achieveNF, constraint }
    
    internal Term GetBodyTerm()
    {
        throw new NotImplementedException();
    }

    internal PlanBody GetBodyNext()
    {
        throw new NotImplementedException();
    }

    internal void SetBodyNext(PlanBody pb)
    {
        throw new NotImplementedException();
    }

    public BodyType getBodyType()
    {
        throw new NotImplementedException();
    }

    public static implicit operator PlanBody(PlanBodyImpl v)
    {
        throw new NotImplementedException();
    }

    internal bool IsEmptyBody()
    {
        throw new NotImplementedException();
    }
}
