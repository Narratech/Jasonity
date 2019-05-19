using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Exceptions;
using Assets.Code.parser;
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

        //Revisar expresión regular, esta es la buena
        //Esta expresión regular es la definición, al menos en Java, de lo que es un identificador 
            //de variable tal y como va a aparecer en las instrucciones de agentSpeak
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

        /*
         @Override
        public Object execute(TransitionSystem ts, Unifier un, Term[] args) throws Exception {
            checkArguments(args);
            StringBuffer sb = new StringBuffer();
            for (Term term : args) {
                if (!term.isString()) {
                    continue;
                }
                StringTerm st = (StringTerm) term;
                Matcher matcher = regex.matcher(st.getString());

                while (matcher.find()) {
                
                    //System.out.println("I found the text \""+matcher.group()+ "\"starting at index "+matcher.start()+ " and ending at index "+matcher.end());
                 
                    String sVar = matcher.group();
                    sVar = sVar.substring(2, sVar.length() - 1); //Quita los dos primeros caracteres: # y \, creo que es lo primero que aparece en las variables; y también quita el último: \
                    try {
                        Term t = null;
                        if (sVar.startsWith("_") && sVar.length() > 1) // deals with unnamed vars, where we cannot use parseTerm
                            t = new UnnamedVar( Integer.valueOf( sVar.substring(1))); //Le quita la _ al nombre de la variable anónima
                        else
                            t = ASSyntax.parseTerm(sVar); //si no ya si que llama al término
                         
                        //We use t.apply to evaluate any logical or arithmetic expression in Jason
                        t = t.capply(un);                    
                        matcher.appendReplacement(sb, t.isString() ? ((StringTerm)t).getString() : t.toString());
                    } catch (ParseException pe) {
                        // TODO: handle exception
                        // TODO: Decide whether or not we should ignore the exception and print the call instead
                        // Right now, if I get a parse error from ASSyntax, I just print the original escaped
                        // sequence, so a user can see that his/her expression was problematic
                        matcher.appendReplacement(sb, "#{"+sVar+"}");
                    }

                }
                matcher.appendTail(sb);
            }

            if (args[args.length - 1].isVar()) {
                //Guardar el mensaje todo ello unificado en una sola variable, para depurar
                StringTerm stRes = new StringTermImpl(sb.toString());
                return un.unifies(stRes, args[args.length - 1]);
            } else {
                //Si no hay una variable, lo que quiero es sacar por pantalla todo el mensaje completo
                ts.getLogger().info(sb.toString()); //Aquí es por donde se saca por la consola lo que el progrmamador haya querido en su asl
                return true;
            }
        }
         */

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
                int last = 0;
                var userBlock = new List<string>();

                foreach (Match item in matcher)
                {
                    //matcher.groups
                    string sVar = item.Groups[0].Value;

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
                        //Hacer Regex.Replace(sb, t.IsString() ?....) serría una opción para hacerlo más compacto pero no sabemos
                        //matcher.appendReplacement(sb, t.IsString() ? ((IStringTerm)t).GetString() : t.ToString());
                       
                        if (sVar.Trim().Length > 0)
                        {
                            userBlock.Add(sVar);
                            //No sabemos si esto está bien: el primer Append es para copiar el trozo 
                                //no matcheado y el segundo para el trozo que sí
                            sb.Append(st.GetString().Substring(last, item.Index - last));
                            sb.Append(t.IsString() ? ((IStringTerm)t).GetString() : t.ToString());
                        }
                        last = item.Index + item.Length;
                    }
                    catch (ParseException pe)
                    {
                        //matcher.appendReplacement(sb, "#{" + sVar + "}");
                        //Creemos que esto es para añadir el trocito que falta (va a estar mal seguro)
                        sb.Append("#{" + sVar + "}");
                    }
                }
                //matcher.appendTail(sb);
                sb.Append(st.GetString().Substring(last));//Aquí hay que añadir lo que falta de la cadena original que no matchea
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
                        int last = 0;
                        var userBlock = new List<string>();

                        foreach (Match item in matcher)
                        {
                            string sVar = item.Groups[0].Value;
                            sVar = sVar.Substring(2, sVar.Length - 1);
                            ITerm v = AsSyntax.AsSyntax.ParseTerm(sVar);
                            if (v.IsVar())
                            {
                                VarTerm to = ((Structure)l).VarToReplace(v, un);
                                //matcher.appendReplacement(sb, "#{" + to.ToString() + "}");
                                if (sVar.Trim().Length > 0)
                                {
                                    userBlock.Add(sVar);
                                    //No sabemos si esto está bien: el primer Append es para copiar el trozo 
                                    //no matcheado y el segundo para el trozo que sí
                                    sb.Append(st.GetString().Substring(last, item.Index - last));
                                    sb.Append(t.IsString() ? ((IStringTerm)t).GetString() : t.ToString());
                                }
                                last = item.Index + item.Length;
                            }
                        }
                        //matcher.appendTail(sb);
                        sb.Append(st.GetString().Substring(last));
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