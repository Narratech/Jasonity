using UnityEngine;
using UnityEditor;
using Assets.Code.BDIAgent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using Assets.Code.Stdlib;

public class light_off : InternalAction
{
    //Tengo que coger de alguna manera la referencia a la lámpara y apagarla
    public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
    {
        return base.Execute(reasoner, un, args);
    }
}