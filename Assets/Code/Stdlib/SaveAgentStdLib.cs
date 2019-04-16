using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using BDIManager.Beliefs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.save_agent</code></b>.
  <p>Description: stores the beliefs, rules, and plans of the agent into a file.
  <p>Parameters:<ul>
  <li>+ file name (atom, string, or variable): the name of the file.
  <li><i>+ initial goals</i> (list -- optional): list of initial goals that will be included in the file.
  </ul>
  <p>Examples:<ul>
  <li> <code>.save_agent("/tmp/x.asl")</code>: save the agent at file "/tmp/x.asl".</li>
  <li> <code>.save_agent("/tmp/x.asl", [start, say(hello)])</code>: includes <code>start</code> and <code>say(hello)</code> as initial goals.</li>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class SaveAgentStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 1;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            string fileName = null;
            if (args[0].IsString())
                fileName = ((IStringTerm)args[0]).GetString();
            else
                fileName = args[0].ToString();

            IListTerm goals = new ListTermImpl();
            if (args.Length > 1)
                goals = (IListTerm)args[1];

            BufferedStream bs = new BufferedStream(new StreamWriter(fileName));

            // store beliefs (and rules)
            bs.Append("// beliefs and rules\n");
            foreach (Literal b in ts.GetAgent().GetBB())
            {
                b = b.Copy();
                b.DelSource(BeliefBase.ASelf);
                bs.Append(b + ".\n");
            }

            // store initial goals
            bs.Append("\n\n// initial goals\n");
            for (ITerm g in goals)
            {
                bs.Append("!" + g + ".\n");
            }


            // store plans
            bs.Append(ts.GetAgent().GetPL().GetAsTxt(false));
            bs.close();
            return true;
        }
    }
}
