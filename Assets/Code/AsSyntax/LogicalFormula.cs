using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic.AsSyntax
{
    interface LogicalFormula:Term, ICloneable
    {
        IEnumerator<Unifier> LogicalConsequence(Agent.Agent ag, Unifier un);
    }
}
