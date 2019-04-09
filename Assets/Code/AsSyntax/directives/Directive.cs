using Assets.Code.AsSyntax;
using Assets.Code.Logic.parser;

namespace Assets.Code.Logic.AsSyntax.directives
{ 
    public interface IDirective {

        void Begin(Pred directive, as2j parser);

        Agent.Agent Process(Pred directive, Agent.Agent outerContent, Agent.Agent innerContent);

        void End(Pred directive, as2j parser);

        bool IsSingleton();
    }
}