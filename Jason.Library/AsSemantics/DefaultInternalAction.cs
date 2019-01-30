using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jason.AsSemantics.AsSemantics
{
    [Serializable]
    public class DefaultInternalAction: InternalAction
    {
        private static readonly long serialVersionUID = 1L;

        public bool suspendIntention()
        {
            return false;
        }
        public bool canBeUsedInContext()
        {
            return true;
        }

        public int getMinArgs()
        {
            return 0;
        }
        public int getMaxArgs()
        {
            return int.MaxValue;
        }

        protected void checkArguments(Term[] args)
        {
        if (args.length<getMinArgs() || args.length > getMaxArgs())
            throw JasonException.createWrongArgumentNb(this);
        }

        public Term[] prepareArguments(Literal body, Unifier un)
        {
            Term[] terms = new Term[body.getArity()];
            for (int i = 0; i < terms.length; i++)
            {
                terms[i] = body.getTerm(i).capply(un);
            }
            return terms;
        }

        public Object execute(TransitionSystem ts, Unifier un, Term[] args) //throws Exception
        {
            return false;
        }

        public void destroy() //throws Exception
        {

        }
    }
}
