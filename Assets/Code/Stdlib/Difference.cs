using Assets.Code.AsSemantic;
using Assets.Code.AsSyntax;
using Assets.Code.Logic.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: S3 is the difference between the sets S1 and S2 (represented by lists).
 * The result set is sorted
 */
namespace Assets.Code.Stdlib
{
    public class Difference: DefaultInternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction create()
        {
            if (singleton == null)
                singleton = new Difference();
            return singleton;
        }

        public int getMinArgs()
        {
            return 3;
        }

        public int getMaxArgs()
        {
            return 3;
        }

        protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);// check number of arguments
            if (!args[0].IsList())
                throw new JasonException.createWrongArgument(this,"first argument '"+args[0]+"'is not a list.");
            if (!args[1].IsList())
                throw new JasonException.createWrongArgument(this,"second argument '"+args[1]+"'is not a list.");
            if (!args[2].IsVar() && !args[2].IsList())
                throw new JasonException.createWrongArgument(this,"last argument '"+args[2]+"'is not a list nor a variable.");
        }

        public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return un.Unifies(args[2], ((IListTerm) args[0]).Difference((IListTerm) args[1]) );
        }
    }
}
