using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.parser;

namespace Assets.Code.AsSyntax.directives
{ 
    public interface IDirective {

        void Begin(Pred directive, as2j parser);

        Agent Process(Pred directive, Agent outerContent, Agent innerContent);

        void End(Pred directive, as2j parser);

        bool IsSingleton();
    }
}