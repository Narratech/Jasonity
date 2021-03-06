﻿
using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: adds an annotation to a Literal
 */
namespace Assets.Code.Stdlib
{
    public class AddAnnotStdLib: InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new AddAnnotStdLib();
            }
            return singleton;
        }

        public override int GetMinArgs()
        {
            return 3;
        }

        public override int GetMaxArgs()
        {
            return 3;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ITerm result = AddAnnotToList(un, args[0], args[1]);
            return un.Unifies(result, args[2]);
        }

        protected ITerm AddAnnotToList(Unifier unif, ITerm l, ITerm annot)
        {
            if (l.IsList())
            {
                IListTerm result = new ListTermImpl();
                foreach (ITerm lTerm in (IListTerm)l)
                {
                    ITerm t = AddAnnotToList(unif, lTerm, annot);
                    if (t != null)
                    {
                        result.Add(t);
                    }
                }
                return result;
            }
            else if (l.IsLiteral())
            {
                return ((Literal)l).ForceFullLiteralImpl().Copy().AddAnnots(annot);
            }
            return l;
        }
    }
}
