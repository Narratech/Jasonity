using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
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
    public class Add_annot: DefaultInternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
            {
                singleton = new Add_annot();
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

        public override object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            Term result = AddAnnotToList(un, args[0], args[1]);
            return un.unifies(result, args[2]);
        }

        protected Term AddAnnotToList(Unifier unif, Term l, Term annot)
        {
            if (l.IsList())
            {
                ListTerm result = new ListTermImpl();
                foreach (Term lTerm in (ListTerm)l)
                {
                    Term t = AddAnnotToList(unif, lTerm, annot);
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
