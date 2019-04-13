using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Puts: InternalAction
    {
        private static InternalAction singleton = null;

        public static InternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new Puts();
            }
            return singleton;
        }

        Regex rx = new Regex(@"#\\{[\\p{Alnum}_]+\\}", RegexOptions.Compiled);

        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        protected override void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args);
            if (!args[0].IsString())
            {
                throw JasonException.CreateWrongArgument(this, "first argument must be a string");
            }
        }

        public object Execute(Reasoner r, Unifier un, ITerm[] args)
        {

        }
    }
}
