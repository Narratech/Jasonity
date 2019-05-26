﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    public class ParserClass
    {
        /*
         * Notes:
         *  -There is just one form of negation in this version of the parser,
         *   unlike the original Jason where exists the Stong negation too.
         *   
         *  -The parser do not interpret lists
        */

        private List<Belief> BeliefsList;
        private List<Desire> DesireList;
        private Dictionary<Term, string> DesireLis;
        private Dictionary<Term, string> PlansList;

        public ParserClass()
        {
            this.BeliefsList = new List<Belief>();
            this.DesireList = new List<Desire>();
            this.PlansList = new Dictionary<Term, string>();
        }

        public void Parser()
        {
            int i = 0;
            Dictionary<int, string> data = new Dictionary<int, string>();

            foreach(string line in File.ReadLines(@"E:\VisualStudio\Pruebas\Pruebas\miner.txt"))
            {
                //The plan, belief or goal definition ends here
                if (line.EndsWith("."))
                {
                    //Adding this last line of definition to the dictionary
                    data.Add(i, line);
                    switch (data.First().Value[0])
                    {
                        //It's a goal
                        case '!':
                            this.DesireList.Add(new Desire(GoalParser(data), "RealObjective"));
                            //Reset all the dictionary data ofr the next iteration 
                            data.Clear();
                            i = 0;
                            break;
                        //It's a test goal
                        case '?':
                            this.DesireList.Add(new Desire(GoalParser(data), "TestObjective"));
                            //Reset all the dictionary data for the next iteration 
                            data.Clear();
                            i = 0;
                            break;
                        //It's a plan for add
                        case '+':
                            //A goal
                            if (data.First().Value.Contains("!"))
                            {
                                this.PlansList.Add(PlanParser(data, true), "RealObjective");
                            }
                            //A test goal
                            else if (data.First().Value.Contains("?"))
                            {
                                this.PlansList.Add(PlanParser(data, true), "TestObjective");
                            }
                            //A belief
                            else
                            {
                                this.PlansList.Add(PlanParser(data, false), "Belief");
                            }
                            break;
                        //It's a plan for delete
                        case '-':
                            //A goal
                            if (data.First().Value.Contains("!"))
                            {
                                this.PlansList.Add(PlanParser(data, true), "RealObjective");
                            }
                            //A test goal
                            else if (data.First().Value.Contains("?"))
                            {
                                this.PlansList.Add(PlanParser(data, true), "TestObjective");
                            }
                            //A belief
                            else
                            {
                                this.PlansList.Add(PlanParser(data, false), "Belief");
                            }
                            break;
                        default:
                            this.BeliefsList.Add(new Belief(BeliefParser(data)));
                            //Reset all the dictionary data ofr the next iteration 
                            data.Clear();
                            i = 0;
                            break;
                    }
                }
                //If the definition hasn't ended yet
                else
                {
                    //The line is added to the dictionary
                    data.Add(i, line);
                    i++;
                }
            }
        }

        /*
            IN: Dictionary
            OUT: Term
        */
        private Term GoalParser(Dictionary<int, string> data)
        {
            string termWithoutSymbol = data.First().Value.Substring(1);
            data.Remove(0);
            data.Add(0, termWithoutSymbol);
            return CommonParser(data);
        }

        /*
            IN: Dictionary
            OUT: Term
        */
        private Term BeliefParser(Dictionary<int, string> data)
        {
            return CommonParser(data);
        }

        /*
            IN: Dictionary
            OUT: Term
        */
        private Term PlanParser(Dictionary<int, string> data, bool isGoal)
        {
            #region TRIGGER

            char @operator = (char)data.First().Value.ToCharArray()[0];
            string s = data.First().Value;
            string triggerForTheParser;
            if (data.First().Value.Contains("<"))
                triggerForTheParser = data.First().Value.Substring(1, data.First().Value.IndexOf("<"));
            else
                triggerForTheParser = data.First().Value.Substring(1, data.First().Value.IndexOf(":"));

            data.Remove(0);

            Dictionary<int, string> aux = new Dictionary<int, string>();
            aux.Add(0, triggerForTheParser);

            Trigger trigger;
            Term t;
            if (isGoal)
                t = GoalParser(aux);
            else
                t = BeliefParser(aux);

            bool belives = true;
            if (data.First().Value.Contains("~"))
                belives = false;
            
            trigger = new Trigger(@operator, belives, (Literal)t);
            aux.Clear();

            #endregion

            #region CONTEXT
            
            //Check if the plan have a context
            if (triggerForTheParser.Contains(":"))
            {
                //It it has one, prepare it for the common parser
                int elementsToErase = 0;
                foreach (KeyValuePair<int, string> entry in data)
                {
                    aux.Add(entry.Key, entry.Value);
                    elementsToErase++;
                    if (data[entry.Key].Contains("<"))
                    {
                        aux[entry.Key] = aux[entry.Key].Substring(0, aux[entry.Key].IndexOf("<") + 1);
                        break;
                    }
                }

                Dictionary<Term, string> context = new Dictionary<Term, string>();

                foreach (KeyValuePair<int, string> entry in aux)
                {
                    Dictionary<int, string> d = new Dictionary<int, string>();
                    d.Add(entry.Key, entry.Value);
                    string s2 = entry.Value.Substring(entry.Value.Length - 1);
                    if (!s2.Equals(";") && !s2.Equals("|") && !s2.Equals(",") && !s2.Equals("&"))
                        context.Add(CommonParser(d), "none");
                    else
                        context.Add(CommonParser(d), s2);
                }

                aux.Clear();

                for (int i = 0; i < elementsToErase; i++)
                    data.Remove(i+1);
            }

            #endregion

            #region PLANBODY

            

            #endregion

            return new Literal("", false);
        }

        /*
            IN: Dictionary
            OUT: Term
        */
        private Term CommonParser(Dictionary<int, string> data)
        {
            //The belief is true by default
            bool beliefThisIsTrue = true, isARule = false;

            Dictionary<Literal, string> terms = new Dictionary<Literal, string>();

            foreach (KeyValuePair<int, string> entry in data)
            {
                string line = entry.Value;

                if (line.IndexOf("~") == 0)
                {
                    beliefThisIsTrue = false;
                    line = line.Substring(1);
                }

                //Check if it's functor
                if (!line.Contains("(") && !line.Contains("["))
                {
                    //Check if it's also a rule
                    if (line.EndsWith(":-"))
                    {
                        isARule = true;
                        terms.Add(new Atom(line.Substring(0, line.IndexOf(":")),
                            beliefThisIsTrue), "");
                    }
                    //Or just a functor
                    else
                    {
                        string @operator = line.Substring(line.Length - 1);
                        if (!@operator.Equals(";") && !@operator.Equals("|") && !@operator.Equals(",") && !@operator.Equals("&"))
                        {
                            @operator = "none";
                        }
                        terms.Add(new Atom(line.Substring(0, line.IndexOf(line.Substring(line.Length - 1))),
                            beliefThisIsTrue), @operator);
                    }
                }
                //Check if it's structure
                else if (line.Contains("(") && !line.Contains("["))
                {
                    //Check if it's also a rule
                    if (line.EndsWith(":-"))
                    {
                        isARule = true;
                        string functor = line.Substring(0, line.IndexOf("("));
                        string parameters = line.Substring(line.IndexOf("(") + 1,
                            line.IndexOf(".") - 1 - (line.IndexOf("(") + 1));
                        terms.Add(new Structure(functor, beliefThisIsTrue, 
                            CheckBrackets(parameters)), "");
                    }
                    //Or jus a structure
                    else
                    {
                        string functor = line.Substring(0, line.IndexOf("("));
                        string @operator = line.Substring(line.Length - 1);
                        string parameters = line.Substring(line.IndexOf("(") + 1,
                            line.IndexOf(@operator) - 1 - (line.IndexOf("(") + 1));
                        if (!@operator.Equals(";") && !@operator.Equals("|") && !@operator.Equals(",") && !@operator.Equals("&"))
                        {
                            @operator = "none";
                        }
                        terms.Add(new Structure(functor, beliefThisIsTrue, 
                            CheckBrackets(parameters)), @operator);
                    }
                }
                //Check if it's predicate
                else
                {
                    //Check if it's also a rule
                    if (line.EndsWith(":-"))
                    {
                        isARule = true;
                        string functor = line.Substring(0, line.IndexOf("("));

                        List<Term> args, annots = new List<Term>();

                        args = CheckBrackets(line.Substring(line.IndexOf("(") + 1,
                            line.IndexOf("[") - 1 - (line.IndexOf("(") + 1)));

                        annots = CheckBrackets(line.Substring(line.IndexOf("[") + 1,
                            line.IndexOf("]") - (line.IndexOf("[") + 1)));

                        terms.Add(new Predicate(functor, beliefThisIsTrue, args, annots), "");
                    }
                    //Or just a predicate
                    else
                    {
                        string functor = line.Substring(0, line.IndexOf("("));

                        List<Term> args, annots = new List<Term>();

                        args = CheckBrackets(line.Substring(line.IndexOf("(") + 1,
                            line.IndexOf("[") - 1 - (line.IndexOf("(") + 1)));

                        annots = CheckBrackets(line.Substring(line.IndexOf("[") + 1,
                            line.IndexOf("]") - (line.IndexOf("[") + 1)));

                        string @operator = line.Substring(line.Length - 1);
                        if (!@operator.Equals(";") && !@operator.Equals("|") && !@operator.Equals(",") && !@operator.Equals("&"))
                        {
                            @operator = "none";
                        }

                        terms.Add(new Predicate(functor, beliefThisIsTrue, args, annots), @operator);
                    }
                }
            }

            //Two ways to return the value, depending if it was a rule
            if (isARule)
            {
                return new Rule(terms, beliefThisIsTrue);
            }
            //Or not
            else
            {
                return terms.First().Key;
            }
        }

        /*
            IN: string
            OUT: List<Term>
        */
        private List<Term> CheckBrackets(string line)
        {
            List<Term> args = new List<Term>();

            //Check if the parameters contains a structure
            if (line.Contains("("))
            {
                //Check if this structure is the first parameter
                if (!line.Substring(0, line.IndexOf("(")).Contains(","))
                {
                    string functor = line.Substring(0, line.IndexOf("("));

                    string parameters = line.Substring
                        (line.IndexOf("(") + 1, line.IndexOf(")") - (line.IndexOf("(") + 1));

                    args.Add(new Structure(functor, true, CheckBrackets(parameters)));
                    //Check if the structure was NOT the last parameter
                    if (line.Substring(line.IndexOf(")")).Contains(","))
                    {
                        args.AddRange(CheckBrackets(line.Substring(line.IndexOf("),") + 2)));
                    }
                    return args;
                }
                //Or it's not the first parameter
                else
                {
                    //Check if the first parameter is a variable
                    if (Char.IsUpper(line, 0) || line.Substring(0, 1) == "_")
                    {
                        args.Add(new Variable(line.Substring(0, line.IndexOf(","))));
                        args.AddRange(CheckBrackets(line.Substring(line.IndexOf(",") + 1)));
                        return args;
                    }
                    //Or an atom
                    else
                    {
                        args.Add(new Atom(line.Substring(0, line.IndexOf(",")), true));
                        args.AddRange(CheckBrackets(line.Substring(line.IndexOf(",") + 1)));
                        return args;
                    }
                }
            }
            //Or do NOT contain a structure
            else
            {
                //Check if the first parameter is a variable
                if (Char.IsUpper(line, 0) || line.Substring(0, 1) == "_")
                {
                    //Check if the variable was NOT the last parameter
                    if (line.Contains(","))
                    {
                        args.Add(new Variable(line.Substring(0, line.IndexOf(","))));
                        args.AddRange(CheckBrackets(line.Substring(line.IndexOf(",") + 1)));
                        return args;
                    }
                    else
                    {
                        args.Add(new Variable(line));
                        return args;
                    }
                }
                //Or an atom
                else
                {
                    //Check if the atom was NOT the last parameter
                    if (line.Contains(","))
                    {
                        args.Add(new Atom(line.Substring(0, line.IndexOf(",")), true));
                        args.AddRange(CheckBrackets(line.Substring(line.IndexOf(",") + 1)));
                        return args;
                    }
                    else
                    {
                        args.Add(new Atom(line, true));
                        return args;
                    }
                }
            }
        }
    }
}