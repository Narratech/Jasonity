using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/** Represents an AgentSpack plan
    (it extends structure to be used as a term)

 @navassoc - label - Pred
 @navassoc - event - Trigger
 @navassoc - context - LogicalFormula
 @navassoc - body - PlanBody
 @navassoc - source - SourceInfo

 */
namespace Jason.Logic.AsSyntax
{
    [Serializable]
    public class Plan : Structure, ICloneable
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
