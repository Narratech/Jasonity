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
        *  -A functor with no parameters still keeps the parenthesis in order 
        *   to simplify the parser
       */

        private Term BeliefsList;
        private Term ObjectivesList;
        private Term PlansList;

        public Parser()
        {

        }

        public void ClassifierParser()
        {

            Dictionary<int, Tuple<string, bool>> data = new Dictionary<int, Tuple<string, bool>>();
            int i = 0;

            foreach (string line in File.ReadLines(@"E:\VisualStudio\Pruebas\Pruebas\Bob.txt"))
            {
                if (line.EndsWith("."))
                    //This means that this line is the end of a rule, objective, belief...
                    data.Add(i, new Tuple<string, bool>(line, true));
                else
                    data.Add(i, new Tuple<string, bool>(line, false));
            }

            while (data.Count() > 0)
            {
                switch (data[0].Item1[0])
                {
                    //Plan: add
                    case '+':

                        break;

                    //Plan: remove
                    case '-':

                        break;

                    //Test goal
                    case '?':

                        break;

                    //Goal
                    case '!':

                        break;

                    //Belief
                    default:
                        ParserBelief(data);
                        //Hacer cosas
                        break;
                }
            }
        }

        private Term ParserBelief(Dictionary<int, Tuple<string, bool>> data)
        {
            bool negation = false;
            string functor;
            //not functor(arguments)
            if (data[0].Item1.Substring(0, 3).Equals("not "))
            {
                negation = true;
                functor = data[0].Item1.Substring(4, data[0].Item1.IndexOf("("));
            }
            //functor(arguments)
            else
            {
                functor = data[0].Item1.Substring(0, data[0].Item1.IndexOf("("));
            }

            if (data[0].Item1.EndsWith("."))
                return new Atom(functor, negation);
            else
            {

            }

        }
    }
}
