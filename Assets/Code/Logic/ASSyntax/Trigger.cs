using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  Represents an AgentSpeak trigger (like +!g, +p, ...).

  It is composed by:
     an operator (+ or -);
     a type (<empty>, !, or ?);
     a literal

  (it extends structure to be used as a term)

  @opt attributes
  @navassoc - literal  - Literal
  @navassoc - operator - TEOperator
  @navassoc - type     - TEType
*/
namespace Jason.Logic.AsSyntax
{
    public class Trigger : Structure, ICloneable
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
