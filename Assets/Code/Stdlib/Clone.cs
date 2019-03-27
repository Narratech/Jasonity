using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
using Assets.Code.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Stdlib
{
    public class Clone: DefaultInternalAction
    {
        public override object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            string agName = args[0].GetString() as StringTerm;
            RuntimeServices services = ts.GetUserAgArch().GetRuntimeServices();
            services.Clone(ts.GetAg(), ts.GetUserAgArch().GetAgArchClassesChain(), agName);

            return true;
        }
    }
}
