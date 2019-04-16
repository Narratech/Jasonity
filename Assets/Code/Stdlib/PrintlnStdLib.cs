using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/**
  <p>Internal action: <b><code>.println</code></b>.
  <p>Description: used for printing messages to the console. Exactly as for
  <code>.print</code> except that a new line is printed after the parameters.
*/

namespace Assets.Code.Stdlib
{
    public class PrintlnStdLib: InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new PrintlnStdLib();
            return singleton;
        }

        protected string GetNewLine()
        {
            return "\n";
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            string sout = ArgsToString(args);

            if (ts != null && ts.GetSettings().LogLevel() != Level.WARNING) {
                ts.GetLogger().info(sout.ToString());
            } else {
                Debug.Log(sout.ToString() + GetNewLine());
            }

            return true;
        }

        protected string ArgsToString(ITerm[] args)
        {
            StringBuilder sout = new StringBuilder();
            //try {
            //    if (ts.getSettings().logLevel() != Level.WARNING && args.length > 0) {
            //        sout = new StringBuilder();
            //    }
            //} catch (Exception e) {}

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].IsString())
                {
                    IStringTerm st = (IStringTerm)args[i];
                    sout.Append(st.GetString());
                }
                else
                {
                    ITerm t = args[i];
                    if (!t.IsVar())
                    {
                        sout.Append(t);
                    }
                    else
                    {
                        sout.Append(t + "<no-value>");
                    }
                }
            }
            return sout.ToString();
        }
    }
}
