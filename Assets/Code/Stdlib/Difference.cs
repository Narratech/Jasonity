using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;

/*
 * Description: S3 is the difference between the sets S1 and S2 (represented by lists).
 * The result set is sorted
 */
namespace Assets.Code.Stdlib
{
    public class Difference: InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new Difference();
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

        protected override void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);// check number of arguments
            if (!args[0].IsList())
                throw JasonityException.CreateWrongArgument(this,"first argument '"+args[0]+"'is not a list.");
            if (!args[1].IsList())
                throw JasonityException.CreateWrongArgument(this,"second argument '"+args[1]+"'is not a list.");
            if (!args[2].IsVar() && !args[2].IsList())
                throw JasonityException.CreateWrongArgument(this,"last argument '"+args[2]+"'is not a list nor a variable.");
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            return un.Unifies(args[2], ((IListTerm) args[0]).Difference((IListTerm) args[1]) );
        }
    }
}
