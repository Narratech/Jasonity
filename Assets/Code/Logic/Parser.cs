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

        private Term BeliefsList;
        private Term ObjectivesList;
        private Term PlansList;

        public Parser()
        {

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
                            break;
                    }
                }
                //La definición de aún no ha terminado
                else
                {
                    //La añado al diccionario
                    data.Add(i, line);
                    i++;
                }

            }
        }



        private Term ParserBelief(Dictionary<int, string> data)
        {
            //The belief is true by default
            bool fake = false;
            string line = data.First().Value;

            //Check if the belief is false
            if (line.IndexOf('t') == 2)
            {
                fake = true;
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
                    return new Atom(line, fake);
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
                    string functor = line.Substring(0, line.IndexOf('('));
                    CheckBrackets(line.Substring(line.IndexOf('(') + 1, line.IndexOf(':') - 1));
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
                    string functor = line.Substring(0, line.IndexOf('('));
                    CheckBrackets(line.Substring(line.IndexOf('(') + 1, line.IndexOf(':') - 1));
                }
            }
        }




        private List<Term> CheckBrackets(string line)
        {
            List<Term> args = new List<Term>();

            //Check if the first argument is not a structure
            if (!line.Contains('('))
            {
                if (!line.Substring(0, line.IndexOf('(')).Contains(','))
                {
                    //Check if it's a variable
                    if (Char.IsUpper(line, 0))
                    {
                        //Check if it's the only argument
                        if (!line.Contains(','))
                        {
                            args.Add(new Var(line.Substring(0, line.IndexOf(')'))));
                            return args;
                        }
                        //Or not
                        else
                        {
                            args.Add(new Var(line.Substring(0, line.IndexOf(','))));
                            args.AddRange(CheckBrackets(line.Substring(line.IndexOf(',') + 1)));
                            return args;
                        }
                    }
                    //Or an atom
                    else
                    {
                        //Check if it's the only argument
                        if (!line.Contains(','))
                        {
                            args.Add(new Atom(line.Substring(0, line.IndexOf(')')), true));
                            return args;
                        }
                        //Or not
                        else
                        {
                            args.Add(new Atom(line.Substring(0, line.IndexOf(')')), true));
                            args.AddRange(CheckBrackets(line.Substring(line.IndexOf(',') + 1)));
                            return args;
                        }
                    }
                }
            }
            //Or it's a structure
            else
            {
                string functor = line.Substring(0, line.IndexOf('('));

                //Check if it's the only argument
                if (!line.Contains(','))
                {
                    args.Add(new Structure(functor, true, CheckBrackets(line.Substring(line.IndexOf('(') + 1))));
                    return args;
                }
                //Or not
                else
                {
                    args.Add(new Structure(functor, true, CheckBrackets(line.Substring(line.IndexOf('(') + 1))));
                    args.AddRange(CheckBrackets(line.Substring(')') + 1));
                    return args;

                }
            }
        }
    }
}
