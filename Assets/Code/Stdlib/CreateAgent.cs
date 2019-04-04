using Assets.Code.Logic;
using Assets.Code.Logic.AsSemantic;
using Assets.Code.Logic.AsSyntax;
using Assets.Code.Mas2J;
using Assets.Code.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description: creates another agent using the referred AgentSpeak source code.
 */
namespace Assets.Code.Stdlib
{
    public class CreateAgent:DefaultInternalAction
    {
        public override int GetMinArgs()
        {
            return 1;
        }

        public override int GetMaxArgs()
        {
            return 2;
        }

        protected override void CheckArguments(Term[] args)//:base.CheckArguments(args)
        {
            if (args.Length > 1 && !args[1].IsString())
            {
                throw JasonException.CreateWrongArgument(this, "second argument must be a string");
            }
            if (args.Length == 3 && !args[2].IsList())
            {
                throw JasonException.CreateWrongArgument(this, "third argument must be a list");

            }
        }

        public override object Execute(Reasoner ts, Unifier un, Term[] args)
        {
            CheckArguments(args);
            string name = GetName(args);
            string source = GetSource(args);

            List<string> AgArchClasses = GetAgArchClasses(args);

            string agClass = null;
            ClassParameters bbPars = null;

            if (args.Length > 2)
            {
                foreach (Term t in args[2] as ListTerm)
                {
                    if (t.IsStructure())
                    {
                        Structure s = t as Structure;
                        if (s.GetFunctor().Equals("beliefBaseClass"))
                        {
                            bbPars = new ClassParameters(TestString(s.GetTerm(0)));
                        }
                        else if (s.GetFunctor().Equals("agentClass"))
                        {
                            agClass = TestString(s.GetTerm(0)).ToString();
                        }
                    }
                }
            }
            RuntimeServices rs = ts.GetUserAgArch().GetRuntimeServices();
            name = rs.CreateAgent(name, source, agClass, AgArchClasses, bbPars, GetSettings(ts), ts.GetAg());
            rs.StartAgent(name);

            if (args[0].IsVar())
            {
                return un.Unifies(new StringTermImpl(name), args[0]);
            }
            else
            {
                return true;
            }
        }

        protected Settings GetName(Term[] args)
        {
            return new Settings();
        }

        protected string GetName(Term[] args)
        {
            string name;
            if (args[0].IsString())
            {
                name = args[0].GetString() as StringTerm;
            }
            else
            {
                name = args[0].ToString();
            }

            if (args[0].IsVar())
            {
                name = name.Substring(0, 1).ToLower() + name.Substring(1);
            }
            return name;
        }

        protected string GetSource(Term[] args)
        {
            string source = null;
            if (args.Length > 1)
            {
                source = args[1].GetString() as StringTerm;
            }
        }

        protected List<string> GetAgArchClasses(Term[] args)
        {
            List<string> AgArchClasses = new List<string>();
            if (args.Length > 2)
            {
                foreach (Term t in args[2] as ListTerm)
                {
                    if (t.IsStructure())
                    {
                        Structure s = t as Structure;
                        if (s.GetFunctor().Equals("agentArchClass"))
                        {
                            AgArchClasses.Add(TestString(s.GetTerm(0)).ToString());
                        }
                    }
                }
            }
            return AgArchClasses;
        }

        private Structure TestString(Term t)
        {
            if (t.IsStructure())
            {
                return t as Structure;
            }
            if (t.IsString())
            {
                return Structure.parse((t as StringTerm).GetString());
            }
        }
    }
}
