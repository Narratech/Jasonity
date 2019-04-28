using Assets.Code.BDIAgent;
using Assets.Code.parser;

namespace Assets.Code.AsSyntax.directives
{
    public class DefaultDirective : IDirective {

        public bool IsSingleton()
        {
            return true;
        }

        public void Begin(Pred directive, as2j parser)
        {

        }

        public virtual Agent Process(Pred directive, Agent outerContent, Agent innerContent)
        {
            return innerContent;
        }

        public void End(Pred directive, as2j parser)
        {

        }
    }
}