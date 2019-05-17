using Assets.Code.Exceptions;
using Assets.Code.AsSyntax.parser;
using Assets.Code.parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace Assets.Code.AsSyntax
{
    public class AsSyntax
    {
        public static readonly string hardDeadLineStr = "hard_deadline";
        private static HashSet<PredicateIndicator> keywords = new HashSet<PredicateIndicator>();

        static AsSyntax() {
            keywords.Add(new PredicateIndicator("atomic", 0));
            keywords.Add(new PredicateIndicator("breakpoint", 0));
            keywords.Add(new PredicateIndicator("all_unifs", 0));
            keywords.Add(new PredicateIndicator("default", 0));

            keywords.Add(new PredicateIndicator("this_ns", 0));

            keywords.Add(new PredicateIndicator("source", 1));
            keywords.Add(new PredicateIndicator("self", 0));
            keywords.Add(new PredicateIndicator("percept", 0));

            keywords.Add(new PredicateIndicator("tell", 0));
            keywords.Add(new PredicateIndicator("untell", 0));
            keywords.Add(new PredicateIndicator("achieve", 0));
            keywords.Add(new PredicateIndicator("unachieve", 0));
            keywords.Add(new PredicateIndicator("askOne", 0));
            keywords.Add(new PredicateIndicator("askAll", 0));
            keywords.Add(new PredicateIndicator("askHow", 0));
            keywords.Add(new PredicateIndicator("tellHow", 0));
            keywords.Add(new PredicateIndicator("untellHow", 0));
        }

        public static void AddKeyword (PredicateIndicator pi)
        {
            keywords.Add(pi);
        }

        public static bool IsKeyword(Literal l)
        {
            return keywords.Contains(l.GetPredicateIndicator());
        }

        public static Literal CreateLiteral (string functor, params ITerm[] t)
        {
            return new LiteralImpl(functor).AddTerms(t);
        }

        public static Literal CreateLiteral(Atom a, string functor, params ITerm[] t)
        {
            return new LiteralImpl(a, Literal.LPos, functor).AddTerms(t); 
        }

        public static Literal CreateLiteral(bool positive, string functor, params ITerm[] t)
        {
            return new LiteralImpl(positive, functor).AddTerms(t);
        }

        public static Literal CreateLiteral(Atom a, bool positive, string functor, params ITerm[] t)
        {
            return new LiteralImpl(a, positive, functor).AddTerms(t);
        }

        public static Structure CreateStructure(string functor, params ITerm[] t)
        {
            int size = (t == null || t.Length == 0 ? 3 : t.Length);
            return (Structure)new Structure(functor, size).AddTerms(t);
        }

        public static Atom CreateAtom(string functor)
        {
            return new Atom(functor);
        }

        public static INumberTerm CreateNumberTerm(double v)
        {
            return new NumberTermImpl(v);
        }

        public static IStringTerm CreateString(string s)
        {
            return new StringTermImpl(s);
        }

        public static IStringTerm CreateString(object o)
        {
            return new StringTermImpl(o.ToString());
        }

        public static VarTerm CreateVar(string functor)
        {
            return new VarTerm(functor);
        }

        public static VarTerm CreateVar(Atom nmspace, string functor) 
        {
            return new VarTerm(nmspace, functor);
        }

        public static VarTerm CreateVar(bool negated, string functor)
        {
            VarTerm v = new VarTerm(functor);
            v.SetNegated(negated);
            return v;
        }

        public static VarTerm CreateVar()
        {
            return new UnnamedVar();
        }

        public static IListTerm CreateList(params ITerm[] terms)
        {
            IListTerm l = new ListTermImpl();
            IListTerm tail = l;
            foreach (ITerm t in terms)
            {
                tail = tail.Append(t);
            }
            return l;
        } 

        public static IListTerm CreateList(List<ITerm> terms)
        {
            IListTerm l = new ListTermImpl();
            IListTerm tail = l;
            foreach (ITerm t in terms)
            {
                tail = tail.Append(t.Clone());
            }
            return l;
        }

        public static Rule CreateRule(Literal head, ILogicalFormula body)
        {
            return new Rule(head, body);
        }

        public static Literal ParseLiteral(string sLiteral)
        {
            as2j parser = new as2j(new StringReader(sLiteral));
            Literal l = parser.literal();
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after "+l+" for parameter '"+sLiteral+"'");
            }  
            return l;
        }

        public static INumberTerm ParseNumber(string sNumber)
        {
            return new NumberTermImpl(double.Parse(sNumber));
        }

        public static Structure ParseStructure(string sStructure)
        {
            as2j parser = new as2j(new StringReader(sStructure));
            ITerm t = parser.term();
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after "+t+" for parameter '"+sStructure+"'");
            }
            if (t.GetType() == typeof(Structure))
            {
                return (Structure) t;
            }
            else
            {
                return new Structure((Literal) t);
            }
        }

        public static VarTerm ParseVar(string sVar)
        { 
            as2j parser = new as2j(new StringReader(sVar));
            VarTerm v = parser.var(Literal.DefaultNS);
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after "+v+" for parameter '"+sVar+"'");
            }
            return v;
        }

        public static ITerm ParseTerm(string sTerm)
        {
            as2j parser = new as2j(new StringReader(sTerm));
            ITerm t = parser.term();
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after "+t+" for parameter '"+sTerm+"'");
            }
            return t;
        }

        public static Plan ParsePlan(string sPlan)
        { 
            as2j parser = new as2j(new StringReader(sPlan));
            Plan p = parser.plan();
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after " + p + " for parameter '" + sPlan + "'");
            }
            return p;
        }

        public static IPlanBody ParsePlanBody(string sPlanBody) 
        { 
            as2j parser = new as2j(new StringReader(sPlanBody));
            IPlanBody p = parser.plan_body();
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after " + p + " for parameter '" + sPlanBody + "'");
            }
            return p;
        }
         
        public static Trigger ParseTrigger(string sTe)
        {
            as2j parser = new as2j(new StringReader(sTe));
            Trigger te = parser.trigger();
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after " + te + " for parameter '" + sTe + "'");
            }
            return te;
        }
         
        public static IListTerm ParseList(string sList)
        {
            as2j parser = new as2j(new StringReader(sList));
            IListTerm l = parser.list();
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after " + l + " for parameter '" + sList + "'");
            }
            return l;
        }

        public static ILogicalFormula ParseFormula(string sExpr)
        {
            as2j parser = new as2j(new StringReader(sExpr));
            ILogicalFormula l = (ILogicalFormula)parser.log_expr();
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after "+l+" for parameter '"+sExpr+"'");
            }
            return l;
        }
         
        public static Rule ParseRule(string sRule)
        {
            as2j parser = new as2j(new StringReader(sRule));
            Rule r = (Rule)parser.belief();
            if (parser.getNextToken().kind != as2jConstants.EOF)
            {
                throw new ParseException("Expected <EOF> after "+r+" for parameter '"+sRule+"'");
            }
            return r;
        }

        public static object TermToObject(ITerm t)
        {
            if (t.IsAtom())
            {
                Atom t2 = (Atom)t;
                if (t2.Equals(Literal.LTrue))
                {
                    return true; //?? Boolean.TRUE
                }
                else if (t2.Equals(Literal.LFalse))
                {
                    return false; //?? Boolean.FALSE
                }
                else
                {
                    return t2.ToString();
                }
            }
            else if (t.IsNumeric())
            {
                INumberTerm nt = (INumberTerm)t;
                double d = 0;
                try
                {
                    d = nt.Solve();
                }
                catch (NoValueException e)
                {
                    //e.printStackTrace();
                }
                if (((byte)d) == d)
                {
                    return (byte)d;
                }
                else if (((int)d) == d)
                {
                    return (int)d;
                }
                else if (((float)d) == d)
                {
                    return (float)d;
                }
                else if (((long)d) == d)
                {
                    return (long)d;
                }
                else
                {
                    return d;
                }
            }
            else if (t.IsString())
            {
                return ((IStringTerm)t).GetString();
            }
            else if (t.IsList())
            {
                List<object> list = new List<object>();
                foreach (ITerm t1 in (IListTerm)t) 
                {
                    list.Add(TermToObject(t1));
                }
                return list;
            }
            else if (t.GetType() == typeof(IObjectTerm))
            {
                return ((IObjectTerm)t).GetObject();
            } else
            {
                return t.ToString();
            }
        }

        public static string GetHardDeadLineStr()
        {
            return hardDeadLineStr;
        }
    }
}
