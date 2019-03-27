using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.Logic;
using UnityEngine;

public class VarTerm : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static explicit operator VarTerm(Term v)
    {
        throw new NotImplementedException();
    }

    internal bool Negated()
    {
        throw new NotImplementedException();
    }

    internal bool IsLiteral()
    {
        throw new NotImplementedException();
    }

    internal bool IsVar()
    {
        throw new NotImplementedException();
    }
}
