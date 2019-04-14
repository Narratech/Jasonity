using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class FailStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new FailStdLib();
            }
            return singleton;
        }

        public override object Execute(Reasoner r, Unifier un, ITerm[] args)
        {
            if (args.Length > 0) //add all arguments as annotations in the exception
            {
                //find message
                ITerm smgs = null;
                string msg = "FailStdLib";
                foreach (ITerm t in args)
                {
                    if (t.IsStructure() && ((Structure)t).GetFunctor().Equals("Error_Msg"))
                    {
                        smgs = t;
                        ITerm tm = ((Structure)t).GetTerm(0);
                        if (tm.IsString())
                        {
                            msg = ((IStringTerm)tm).GetString();
                        }
                        else
                        {
                            msg = tm.ToString();
                        }
                        break;
                    }
                }

                JasonityException e = new JasonityException(msg);
                foreach (ITerm t in args)
                {
                    if (t != smgs)
                    {
                        e.AddErrorAnnot(t);
                    }
                }
                throw e;
            }
            return false;
        }
    }
}
