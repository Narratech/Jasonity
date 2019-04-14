using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;
using Assets.Code.Logic;
using UnityEngine;

namespace Assets.Code.Logic
{
    // Uses EqualsAsTerm for equals
    public class StructureWrapperForLiteral
    {
        private Literal l;

        public StructureWrapperForLiteral(Literal l) => this.l = l;

        public int HashCode() => l.GetHashCode();

        public override bool Equals(object o) => o is StructureWrapperForLiteral && l.EqualsAsStructure(((StructureWrapperForLiteral)o).l);

        public override string ToString() => l.ToString();

        public int CompareTo(StructureWrapperForLiteral o) => l.CompareTo(o.l);

        public Literal GetLiteral() => l;
    }
}

