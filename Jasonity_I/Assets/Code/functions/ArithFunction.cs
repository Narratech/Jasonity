﻿using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;

namespace Assets.Code.functions
{
    public class ArithFunction
    {
        public virtual string GetName() => GetType().Name; 

        public virtual bool AllowUngroundTerms() => false;

        public virtual bool CheckArity(int a) => true;

        public virtual double Evaluate(Reasoner r, ITerm[] args) => 0;

        public override string ToString() => "function " + GetName();
    }
}