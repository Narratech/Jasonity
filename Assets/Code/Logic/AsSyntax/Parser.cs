using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Parser
    {
        /*
         * Notes:
         *  -There is just one form of negation in this version of the parser,
         *   unlike the original Jason where there are two
         *   
         *   -The parser do not interpret lists
        */

        private List<DefaultTerm> BeliefsList;
        private Dictionary<DefaultTerm, string> GoalList;
        private List<DefaultTerm> PlansList;

        public Parser()
        {
            this.BeliefsList = new List<DefaultTerm>();
            this.GoalList = new Dictionary<DefaultTerm, string>();
            this.PlansList = new List<DefaultTerm>();
        }

        public void ClassifierParser()
        {
            int i = 0;
            Dictionary<int, string> data = new Dictionary<int, string>();

            foreach (string line in File.ReadLines(@"E:\VisualStudio\Pruebas\Pruebas\miner.txt"))
            {
                //La creencia, objetivo o plan ha terminado su definición
                if (line.EndsWith("."))
                {
                    //Añado la última líne de la definición al diccionario
                    data.Add(i, line);
                    switch (data.First().Value[0])
                    {
                        //The goal
                        case '!':
                            this.GoalList.Add(GoalParser(data), "Real");
                            //Reseteo los datos del diccionario 
                            //para la próxima definición
                            data.Clear();
                            i = 0;
                            break;
                        case '?':
                            this.GoalList.Add(GoalParser(data), "Test");
                            //Reseteo los datos del diccionario 
                            //para la próxima definición
                            data.Clear();
                            i = 0;
                            break;
                        case '+':

                            break;
                        case '-':

                            break;
                        default:
                            this.BeliefsList.Add(BeliefParser(data));
                            //Reseteo los datos del diccionario 
                            //para la próxima definición
                            data.Clear();
                            i = 0;
                            break;
                    }
                }
                //La definición de aún no ha terminado
                else
                {
                    //La añado al diccionario
                    /*
                     Tengo que montarme algo para ignorar espacios en Blanco
                     */
                    data.Add(i, line);
                    i++;
                }
            }
        }

        /*
            IN: Dictionary
            OUT: Term
        */
        private DefaultTerm GoalParser(Dictionary<int, string> data)
        {
            string justTheLiteral = data.First().Value.Substring(1);
            data.Remove(0);
            data.Add(0, justTheLiteral);
            return CommonParser(data);
        }

        /*
            IN: Dictionary
            OUT: Term
        */
        private DefaultTerm BeliefParser(Dictionary<int, string> data)
        {
            return CommonParser(data);
        }

        /*
            IN: Dictionary
            OUT: Term
        */
        private DefaultTerm CommonParser(Dictionary<int, string> data)
        {
            //The belief is true by default
            bool notFake = true, isARule = false;

            Dictionary<Literal, string> terms = new Dictionary<Literal, string>();

            foreach (KeyValuePair<int, string> entry in data)
            {
                string line = entry.Value;

                if (line.IndexOf("not ") == 0)
                {
                    notFake = false;
                }

                //Check if it's functor
                if (!line.Contains("(") && !line.Contains("["))
                {
                    //Check if it's a rule
                    if (line.EndsWith(":-"))
                    {
                        isARule = true;
                        terms.Add(new Atom(line.Substring(0, line.IndexOf(":")), notFake), "");
                    }
                    //Or just a functor
                    else
                    {
                        string @operator = line.Substring(line.Length - 1);
                        terms.Add(new Atom(line.Substring(0, line.IndexOf(@operator)), notFake), @operator);
                    }
                }
                //Check if it's structure
                else if (line.Contains("(") && !line.Contains("["))
                {
                    //Check if it's a rule
                    if (line.EndsWith(":-"))
                    {
                        isARule = true;
                        string functor = line.Substring(0, line.IndexOf("("));
                        string parameters = line.Substring(line.IndexOf("(") + 1,
                            line.IndexOf(".") - 1 - (line.IndexOf("(") + 1));
                        terms.Add(new Structure(functor, notFake, CheckBrackets(parameters)), "");
                    }
                    //Or jus a structure
                    else
                    {
                        string functor = line.Substring(0, line.IndexOf("("));
                        string @operator = line.Substring(line.Length - 1);
                        string parameters = line.Substring(line.IndexOf("(") + 1,
                            line.IndexOf(@operator) - 1 - (line.IndexOf("(") + 1));
                        terms.Add(new Structure(functor, notFake, CheckBrackets(parameters)), @operator);
                    }
                }
                //Check if it's predicate
                else
                {
                    //Check if it's a rule
                    if (line.EndsWith(":-"))
                    {
                        isARule = true;
                        string functor = line.Substring(0, line.IndexOf("("));

                        List<DefaultTerm> args, annots = new List<DefaultTerm>();

                        args = CheckBrackets(line.Substring(line.IndexOf("(") + 1,
                            line.IndexOf("[") - 1 - (line.IndexOf("(") + 1)));

                        annots = CheckBrackets(line.Substring(line.IndexOf("[") + 1,
                            line.IndexOf("]") - (line.IndexOf("[") + 1)));

                        terms.Add(new Predicate(functor, notFake, args, annots), "");
                    }
                    //Or just a predicate
                    else
                    {
                        string functor = line.Substring(0, line.IndexOf("("));

                        List<DefaultTerm> args, annots = new List<DefaultTerm>();

                        args = CheckBrackets(line.Substring(line.IndexOf("(") + 1,
                            line.IndexOf("[") - 1 - (line.IndexOf("(") + 1)));

                        annots = CheckBrackets(line.Substring(line.IndexOf("[") + 1,
                            line.IndexOf("]") - (line.IndexOf("[") + 1)));

                        terms.Add(new Predicate(functor, notFake, args, annots), line.Substring(line.Length - 1));
                    }
                }
            }

            //Two ways to return the value, depending if it was a rule
            if (isARule)
            {
                return new Rule(terms, notFake);
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
        private List<DefaultTerm> CheckBrackets(string line)
        {
            List<DefaultTerm> args = new List<DefaultTerm>();

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
