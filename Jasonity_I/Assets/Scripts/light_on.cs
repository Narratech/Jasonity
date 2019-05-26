using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using Assets.Code.Stdlib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class light_on : InternalAction
{
    public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
    {
        //Aqui tengo que coger, de alguna manera, la referencia a la lámpara y encender la luz
        return base.Execute(reasoner, un, args);
    }
}
