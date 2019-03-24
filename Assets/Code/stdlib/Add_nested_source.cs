using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Code.stdlib
{
    public class Add_nested_source:DefaultInternalAction
    {
        private static InternalAction singleton = null;

        private static InternalAction create()
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

        public object execute(TransitionSystem ts, Unifier un, DefaultTerm[] args)
        {
            CheckArguments(args);
            try
            {
                return un.unifies(addAnnotToList(args[0], args[1], args[2]));
            }
            catch (Exception e)
            {
                throw new Exception("Error adding nest source '"+args[1]+"' to"+args[0], e);
            }
        }

        public static DefaultTerm addAnnotToList(DefaultTerm l, DefaultTerm source)
        {
            if (l.IsList())
            {
                ListTerm result = new ListTermImpl();
                foreach (Term lTerm in (ListTerm)l)
                {
                    Term t = addAnnotToList(lTerm, source);
                    if (t != null)
                    {
                        result.add(t);
                    }
                }
                return result;
            }
            else if (l.IsLiteral())
            {
                Literal result = ((Literal)l).forceFullLiteralImpl().copy();

                //Create the source annots
                Literal ts = Predicate.createSource(source).addAnnots(result.getAnnots("source"));

                result.delSources();
                result.addAnnots(ts);
                return result;
            }
            else
            {
                return l;
            }
        }
    }
}
