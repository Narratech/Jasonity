using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: concatenates strings or lists.
 */
namespace Assets.Code.Stdlib
{
    public class ConcatStdLib: InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if(singleton == null)
            {
                singleton = new ConcatStdLib();
            }
            return singleton;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            if (args[0].IsList())
            {
                if (!args[args.Length-1].IsVar() && !args[args.Length-1].IsList())
                {
                    throw new JasonityException("Last argument of concat '" + args[args.Length - 1] + "'is not a list nor a variable.");
                }
                IListTerm result = args[0].Clone() as IListTerm;
                for (int i = 1; i < args.Length - 1; i++)
                {
                    if (!args[i].IsList())
                    {
                        throw JasonityException.CreateWrongArgument(this, "arg[" + i + "] is not a list");
                    }
                    result.Concat((IListTerm)args[i].Clone());   
                }
                return un.Unifies(result, args[args.Length - 1]);
            }
            else
            {
                if (!args[args.Length - 1].IsVar() && !args[args.Length - 1].IsString())
                {
                    throw JasonityException.CreateWrongArgument(this, "Last argument '" + args[args.Length - 1] + "' is not a string nor a variable.");
                }
                string vl = args[0].ToString();
                if (args[0].IsString())
                {
                    vl = ((IStringTerm)args[0]).GetString();
                }
                StringBuilder sr = new StringBuilder(vl);
                for (int i = 0; i < args.Length-1; i++)
                {
                    vl = args[i].ToString();
                    if (args[i].IsString())
                    {
                        vl = ((IStringTerm)args[i]).GetString();
                    }
                    sr.Append(vl);
                }
                return un.Unifies(new StringTermImpl(sr.ToString()), args[args.Length-1]);
            }
        }
    }
}
