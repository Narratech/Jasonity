using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.Logic.AsSyntax;

namespace Assets.Code.Logic.AsSemantic
{
    [Serializable]
    public class DefaultInternalAction : InternalAction
    {
        private static readonly long serialVersion = 1L;

        public bool SuspendIntention()
        {
            return true;
        }

        public bool CanBeUsedInContext()
        {
            return true;
        }

        public void Destroy()
        {
            
        }

        public int GetMinArgs()
        {
            return 0;
        }

        public int GetMaxArgs()
        {
            return Int32.MaxValue;
        }

        protected void CheckArguments(Term[] args)
        {
            if (args.Length < GetMinArgs() || args.Length > GetMaxArgs())
            {
                throw JasonException.CreateWrongArgumentsNb(this);
            }
        }

        public object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            return false;
        }

        public Term[] PrepareArguments(Literal body, Unifier un)
        {
            Term[] terms = new Term[body.GetArity()];
            for (int i = 0; i < terms.Length; i++)
            {
                terms[i] = terms.GetTerm(i).Capply(un);
            }
            return terms;
        }
    }
}
