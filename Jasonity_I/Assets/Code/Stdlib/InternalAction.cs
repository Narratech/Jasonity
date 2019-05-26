using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;

namespace Assets.Code.Stdlib
{
    public class InternalAction
    {
        public virtual bool SuspendIntention()
        {
            return false;
        }
        public virtual bool CanBeUsedInContext()
        {
            return true;
        }

        public virtual int GetMinArgs()
        {
            return 0;
        }
        public virtual int GetMaxArgs()
        {
            return int.MaxValue;
        }

        protected virtual void CheckArguments(ITerm[] args) 
        {
            if (args.Length<GetMinArgs() || args.Length > GetMaxArgs())
            {
                throw JasonityException.CreateWrongArgumentNb(this);
            }
        }

        public virtual ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            ITerm[] terms = new ITerm[body.GetArity()];
            for (int i = 0; i < terms.Length; i++)
            {
                terms[i] = body.GetTerm(i).CApply(un);
            }
            return terms;
        }

        public virtual object Execute(Reasoner reasoner, Unifier un, ITerm[] args) 
        {
            return false;
        }

        public void Destroy()
        {

        }
    }
}
