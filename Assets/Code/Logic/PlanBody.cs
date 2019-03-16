using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.Logic;
using UnityEngine;

public class PlanBody : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    internal object getBodyType()
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
