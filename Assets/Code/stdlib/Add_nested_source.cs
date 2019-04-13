using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: adds a source annotation to a literal (used in communication)
 */
namespace Assets.Code.Stdlib
{
    public class Add_nested_source:DefaultInternalAction
    {
        private static IInternalAction singleton = null;

        private static IInternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new Add_nested_source();
            }
            return singleton;
        }

        public int GetMinArgs()
        {
            return 3;
        }

        public int GetMaxArgs()
        {
            return 3;
        }

        public object Execute(Reasoner ts, Unifier un, DefaultTerm[] args)
        {
            CheckArguments(args);
            try
            {
                return un.Unifies(AddAnnotToList(args[0], args[1], args[2]));
            }
            catch (Exception e)
            {
                throw new Exception("Error adding nest source '"+args[1]+"' to"+args[0], e);
            }
        }

        public static ITerm AddAnnotToList(DefaultTerm l, DefaultTerm source)
        {
            if (l.IsList())
            {
                IListTerm result = new ListTermImpl();
                foreach (ITerm lTerm in (IListTerm)l)
                {
                    ITerm t = AddAnnotToList(lTerm, source);
                    if (t != null)
                    {
                        result.Add(t);
                    }
                }
                return result;
            }
            else if (l.IsLiteral())
            {
                Literal result = ((Literal)l).ForceFullLiteralImpl().Copy();

                //Create the source annots
                Literal ts = Pred.CreateSource(source).AddAnnots(result.GetAnnots("source"));

                result.DelSources();
                result.AddAnnots(ts);
                return result;
            }
            else
            {
                return l;
            }
        }
    }
}
