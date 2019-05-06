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

            StreamWriter bs = new StreamWriter(new FileStream(fileName, FileMode.Open)); // Not sure if this works, but it doesn't give an error
            // ORIGINAL: BufferedWriter bs = new BufferedWriter(new FileWriter(fileName));

            // store beliefs (and rules)
            //Hemos cambiado todos los Append por Write
            bs.Write("// beliefs and rules\n");
            foreach (Literal b in ts.GetAgent().GetBB())
            {
                //b = b.copy();
                //b.delSource(BeliefBase.ASelf);
                //out.append(b + ".\n");
                var baux = b.Copy();
                baux.DelSource(BeliefBase.ASelf);
                bs.Write(baux + ".\n");
            }

            // store initial goals
            bs.Write("\n\n// initial goals\n");
            foreach (ITerm g in goals)
            {
                bs.Write("!" + g + ".\n");
            }


            // store plans
            bs.Write(ts.GetAgent().GetPL().GetAsTxt(false));
            bs.Close();
            return true;
        }
    }
}
