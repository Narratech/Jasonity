using Assets.Code.Agent;
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Stdlib
{
    public class PutsStdLib : InternalAction
    {
        private static InternalAction singleton = null;

        public static InternalAction Create()
        {
            if (singleton == null)
            {
                singleton = new PutsStdLib();
            }
            return singleton;
        }

        //Revisar expresión regular
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
                throw JasonityException.CreateWrongArgument(this, "first argument must be a string");
            }
        }

        public override object Execute(Reasoner r, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            StringBuilder sb = new StringBuilder();
            foreach (ITerm term in args)
            {
                if (!term.IsString())
                {
                    continue;
                }
                IStringTerm st = (IStringTerm)term;
                MatchCollection matcher = rx.Matches(st.GetString());

                foreach (Match item in matcher)
                {
                    //matcher.groups
                    string sVar = item.ToString();

                    sVar = sVar.Substring(2, sVar.Length - 1);
                    try
                    {
                        ITerm t = null;
                        if (sVar.StartsWith("_") && sVar.Length > 1)
                        {
                            t = new UnnamedVar(int.Parse(sVar.Substring(1)));
                        }
                        else
                        {
                            t = AsSyntax.AsSyntax.ParseTerm(sVar);
                        }
                        t = t.Capply(un);
                        //Hacer Regex.Replace(sb, t.IsString() ?....)
                        matcher.appendReplacement(sb, t.IsString() ? ((IStringTerm)t).GetString() : t.ToString());
                    }
                    catch (ParseException pe)
                    {
                        matcher.appendReplacement(sb, "#{" + sVar + "}");
                    }
                }
                matcher.ppendTail(sb);
            }

            if (args[args.Length - 1].IsVar())
            {
                IStringTerm stRes = new StringTermImpl(sb.ToString());
                return un.Unifies(stRes, args[args.Length - 1]);
            }
            else
            {
                //r.GetLogger().Info(sb.ToString());
                return true;
            }
        }

        public void MakeVarsAnnon(Literal l, Unifier un)
        {
            try
            {
                for (int i = 0; i < l.GetArity(); i++)
                {
                    ITerm t = l.GetTerm(i);
                    if (t.IsString())
                    {
                        IStringTerm st = (IStringTerm)t;
                        MatchCollection matcher = rx.Matches(st.GetString());
                        StringBuilder sb = new StringBuilder();

                        foreach (Match item in matcher)
                        {
                            string sVar = matcher.group();
                            sVar = sVar.Substring(2, sVar.Length - 1);
                            ITerm v = AsSyntax.AsSyntax.ParseTerm(sVar);
                            if (v.IsVar())
                            {
                                VarTerm to = ((Structure)l).VarToReplace(v, un);
                                matcher.appendReplacement(sb, "#{" + to.ToString() + "}");
                            }
                        }
                        matcher.appendTail(sb);
                        l.SetTerm(i, new StringTermImpl(sb.ToString()));
                    }
                }
            }
            catch (ParseException pe)
            {
                Debug.Log(pe.ToString());
            }
        }
    }
}