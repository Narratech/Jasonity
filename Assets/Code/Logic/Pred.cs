using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.Logic;
using Assets.Code.ReasoningCycle;
using UnityEngine;

public class Pred : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static explicit operator Pred(Term v)
    {
        throw new NotImplementedException();
    }

    internal bool IsVar()
    {
        throw new NotImplementedException();
    }

    internal bool HasAnnot()
    {
        throw new NotImplementedException();
    }

    internal bool HasSubsetAnnot(Pred np2, Unifier unifier)
    {
        throw new NotImplementedException();
    }

    internal void ClearAnnots()
    {
        throw new NotImplementedException();
    }

    internal bool IsLiteral()
    {
        throw new NotImplementedException();
    }

    internal bool Negated()
    {
        throw new NotImplementedException();
    }

    internal Literal Clone()
    {
        throw new NotImplementedException();
    }

    public static explicit operator Pred(Literal v)
    {
        throw new NotImplementedException();
    }

    internal void SetNegated(object lPos)
    {
        throw new NotImplementedException();
    }
}
