using Assets.Code.BDIAgent;
using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using UnityEngine;

/**
  <p>Internal action: <b><code>.printf(format, args...)</code></b>.
  <p>Description: used for printing messages to the console inspired by Java printf/format.
  NB.: do not use "%d" since all numbers used by this internal action are translated from Jason to a Java double.
    
  <p>Examples:<ul>
  <li> <code>.printf("Value %08.0f%n",N)</code>: prints <code>Value 00461012</code>, when N is 461012.</li>
  <li> <code>.printf("Value %10.3f",N)</code>: prints <code>Value      3.142</code>, when N is 3.14159.</li>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class PrintfStdLib:PrintStdLib
    {
        private static InternalAction singleton = null;

        public static InternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new PrintfStdLib();
            }
            return singleton;
        }

        protected override void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); //check number of arguments
            if (!args[0].IsString())
            {
                throw JasonityException.CreateWrongArgument(this, "first argument must be a string (the format)");
            }
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            object[] cSharpArgs = new object[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
            {
                cSharpArgs[i - 1] = AsSyntax.AsSyntax.TermToObject(args[i]);
                if (args[i].IsNumeric())
                { // no integers! (since .printf("%d",10.2) and .pring("%.2f",10) produces erros
                    cSharpArgs[i - 1] = 0;
                    try
                    {
                        cSharpArgs[i - 1] = ((INumberTerm)args[i]).Solve();
                    }
                    catch (NoValueException e)
                    {
                        e.ToString();
                    }
                }
            }
            string sout = string.Format(((IStringTerm)args[0]).GetString(), cSharpArgs);

            //if (ts != null && ts.GetSettings().LogLevel() != Level.WARNING)
            //{
            //    ts.GetLogger().info(sout.ToString());
            //}
            //else
            //{
            //    Debug.Log(sout.ToString() + GetNewLine());
            //}

            return true;
        }
    }
}
