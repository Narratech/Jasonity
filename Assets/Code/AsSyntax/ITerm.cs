﻿using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;

namespace Assets.Code.AsSyntax
{

    public interface ITerm: IComparable<ITerm>
    {
        bool IsVar();
        bool IsUnnamedVar();
        bool IsLiteral();
        bool IsRule();
        bool IsList();
        bool IsString();
        bool IsInternalAction();
        bool IsArithExpr();
        bool IsNumeric();
        bool IsPred();
        bool IsGround();
        bool IsStructure();
        bool IsAtom();
        bool IsPlanBody();
        bool IsCyclicTerm();

        bool HasVar(VarTerm t, Unifier u);
        VarTerm GetCyclicVar();

        void CountVars(Dictionary<VarTerm, int?> c);

        new ITerm Clone();

        bool Equals(object o);

        bool Subsumes(ITerm l);

        /** clone and applies together (and faster than clone and then apply) */
        ITerm Capply(Unifier u);

        /** clone in another namespace */
        ITerm CloneNS(Atom Newnamespace);

        void SetSrcInfo(SourceInfo s);
        SourceInfo GetSrcInfo();
    }
}
