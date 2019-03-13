using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
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

        private List<Term> BeliefsList;
        private List<Term> ObjectivesList;
        private List<Term> PlansList;

        public Parser()
        {
            this.BeliefsList = new List<Term>();
            this.ObjectivesList = new List<Term>();
            this.PlansList = new List<Term>();
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
                        case '!':
                            break;
                        case '?':
                            break;
                        case '+':
                            break;
                        case '-':
                            break;
                        default:
                            ParserBelief(data);

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



        private void ParserBelief(Dictionary<int, string> data)
        {
            //The belief is true by default
            bool notFake = true;
            string line = data.First().Value;

            //Check if the belief is false
            if (line.IndexOf('t') == 2)
            {
                notFake = true;
            }

            //Check if it's functor
            if (!line.Contains("(") && !line.Contains("["))
            {
                //Check if it's a rule
                if (line.EndsWith(":-"))
                {

                }
                //Or just a functor
                else
                {
                    this.BeliefsList.Add(new Atom(line.Substring(0, line.IndexOf(".") - 1), notFake));
                }
            }
            //Check if it's structure
            else if (line.Contains("(") && !line.Contains("["))
            {
                //Check if it's a rule
                if (line.EndsWith(":-"))
                {

                }
                else
                {
                    string functor = line.Substring(0, line.IndexOf("("));
                    string parameters = line.Substring
                        (line.IndexOf("(") + 1, line.IndexOf(".") - 1 - (line.IndexOf("(") + 1));
                    this.BeliefsList.Add(new Structure(functor, notFake, CheckBrackets(parameters)));
                }
            }
            //Check if it's predicate
            else
            {
                //Check if it's a rule
                if (line.EndsWith(":-"))
                {

                }
                else
                {
                    string functor = line.Substring(0, line.IndexOf("("));
                    this.BeliefsList.Add(new Predicate(functor, notFake,
                        CheckBrackets(line.Substring(line.IndexOf('(') + 1, line.IndexOf(')') - 1))));
                }
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
                    if (Char.IsUpper(line, 0))
                    {
                        args.Add(new Var(line.Substring(0, line.IndexOf(","))));
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
                if (Char.IsUpper(line, 0))
                {
                    //Check if the variable was NOT the last parameter
                    if (line.Contains(","))
                    {
                        args.Add(new Var(line.Substring(0, line.IndexOf(","))));
                        args.AddRange(CheckBrackets(line.Substring(line.IndexOf(",") + 1)));
                        return args;
                    }
                    else
                    {
                        args.Add(new Var(line));
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
