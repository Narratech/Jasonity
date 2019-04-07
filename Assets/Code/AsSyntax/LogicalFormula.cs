using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    interface ILogicalFormula:ITerm, ICloneable
    {
        IEnumerator<Unifier> LogicalConsequence(Agent.Agent ag, Unifier un);
    }
}
