using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Code.Logic.AsSemantic
{
    public interface InternalAction
    {
        /** Returns true if the internal action (IA) should suspend the
        intention where the IA is called */
        bool SuspendIntention();

        /** Return true if the internal action can be used in plans' context */
        bool CanBeUsedInContext();

        /** Prepare body's terms to be used in 'execute', normally it consist of cloning and applying each term */
        Term[] PrepareArguments(Literal body, Unifier un);

        /** Executes the internal action. It should return a Boolean or
         *  an Iterator<Unifier>. A true boolean return means that the IA was
         *  successfully executed. An Iterator result means that there is
         *  more than one answer for this IA (e.g. see member internal action). */
        object Execute(Reasoner ts, Unifier un, Term[] args);

        void Destroy();
    }
}
