using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.Mas2J;
using Assets.Code.ReasoningCycle;
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

        protected override void CheckArguments(ITerm[] args)//:base.CheckArguments(args)
        {
            if (args.Length > 1 && !args[1].IsString())
            {
                throw JasonityException.CreateWrongArgument(this, "second argument must be a string");
            }
            if (args.Length == 3 && !args[2].IsList())
            {
                throw JasonityException.CreateWrongArgument(this, "third argument must be a list");

            }
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            string name = GetName(args);
            string source = GetSource(args);

            List<string> AgArchClasses = GetAgArchClasses(args);

            string agClass = null;
            ClassParameters bbPars = null;

            if (args.Length > 2)
            {
                foreach (ITerm t in args[2] as IListTerm)
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
            name = rs.CreateAgent(name, source, agClass, AgArchClasses, bbPars, GetSettings(ts), ts.GetAgent());
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

        protected Settings GetName(ITerm[] args)
        {
            return new Settings();
        }

        protected string GetName(ITerm[] args)
        {
            string name;
            if (args[0].IsString())
            {
                name = args[0].GetString() as IStringTerm;
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

        protected string GetSource(ITerm[] args)
        {
            string source = null;
            if (args.Length > 1)
            {
                source = args[1].GetString() as IStringTerm;
            }
        }

        protected List<string> GetAgArchClasses(ITerm[] args)
        {
            List<string> AgArchClasses = new List<string>();
            if (args.Length > 2)
            {
                foreach (ITerm t in args[2] as IListTerm)
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

        private Structure TestString(ITerm t)
        {
            if (t.IsStructure())
            {
                return t as Structure;
            }
            if (t.IsString())
            {
                return AsSyntax.AsSyntax.ParseStructure((t as IStringTerm).GetString());
            }
        }
    }
}
