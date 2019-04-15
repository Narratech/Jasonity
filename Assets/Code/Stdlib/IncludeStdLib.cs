using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class IncludeStdLib:InternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        Atom ns = Literal.DefaultNS;

        public override ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            ns = body.GetNS();
            return base.PrepareArguments(body, un);
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            CheckArguments(args);

            Agent.Agent ag = reasoner.GetAgent();
            Pred inc = new Pred(ns, "include");
            inc.AddTerms(args);

            //-Cosas Java???
            //-Sí Peterrr, cÓsas Naz* ¡digo! cÓsas Java
            Agent.Agent result = ((IncludeStdLib)DirectiveProcessor.getDirective("include")).process(inc, ag, null);

            ag.ImportComponents(result);
            ag.AddInitialBelsInBB();
            ag.AddInitialDesiresInReasoner();

            if (args.Length > 1 && args[1].IsVar())
            {
                return un.Unifies(args[1], inc.GetTerm(1));
            }
            else
            {
                return true;
            }
        }
    }
}
