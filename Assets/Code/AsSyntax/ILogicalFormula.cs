using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    public interface ILogicalFormula : ITerm
    {
        IEnumerator<Unifier> LogicalConsequence(Agent ag, Unifier un);
    }
}
