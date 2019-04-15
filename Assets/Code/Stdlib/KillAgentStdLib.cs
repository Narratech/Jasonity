
using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
  <p>Internal action: <b><code>.kill_agent</code></b>.
  <p>Description: kills the agent whose name is given as parameter.This is a
    provisional internal action to be used while more adequate mechanisms for
     creating and killing agents are being developed.In particular, note that
    an agent can kill any other agent, without any consideration on
    permissions, etc.! It is the programmers' responsibility to use this
     action.
*/

namespace Assets.Code.Stdlib
{
    public class KillAgentStdLib:InternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 1;
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            string name;
            if (args[0].IsString())
                name = ((IStringTerm)args[0]).GetString();
            else
                name = args[0].ToString();
            return reasoner.GetUserAgArch().GetRuntimeServices().KillAgent(name, reasoner.GetUserAgArch().GetAgentName());
        }
    }
}
