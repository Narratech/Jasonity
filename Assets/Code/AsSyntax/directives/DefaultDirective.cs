using Assets.Code.AsSyntax;
using Assets.Code.Logic.parser;

namespace Assets.Code.Logic.AsSyntax.directives
{ 
    public class DefaultDirective : IDirective {

        public bool IsSingleton()
        {
            return true;
        }

        public void Begin(Pred directive, as2j parser)
        {

        }

        public Agent.Agent Process(Pred directive, Agent.Agent outerContent, Agent.Agent innerContent)
        {
            return innerContent;
        }

        public void End(Pred directive, as2j parser)
        {

        }
    }
}