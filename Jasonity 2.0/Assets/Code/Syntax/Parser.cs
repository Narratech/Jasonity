using Assets.Code.BDI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Syntax
{
    public class Parser
    {
        private List<Belief> beliefList;
        private List<Desire> desireList;
        private List<Plan> planList;

        public List<Belief> GetBeliefs()
        {
            return beliefList;
        }

        public List<Desire> GetDesires()
        {
            return desireList;
        }

        public List<Plan> GetPlans()
        {
            return planList;
        }

        public Parser()
        {
            beliefList = new List<Belief>();
            desireList = new List<Desire>();
            planList = new List<Plan>();
        }

        public void Parse(string route)
        {
            List<string> data = new List<string>();
            foreach(string line in File.ReadLines(@route))
            {
                if (line.EndsWith("."))
                {
                    data.Add(line);
                    switch (data.First().ElementAt(0))
                    {
                        //It's a trap, i mean, a goal
                        case '!':
                            break;
                        //It's a test goal
                        case '?':
                            break;
                        //It's a plan for add
                        case '+':
                            //If it's a goal
                            if (data.First().Contains("!"))
                            {

                            }
                            //If it's a test goal
                            else if(data.First().Contains("?"))
                            {

                            }
                            //It it's a belief
                            else
                            {
                                planList.Add(ParsePlan(data));
                            }
                            break;
                        //It's a plan for delete
                        case '-':
                            //If it's a goal
                            if (data.First().Contains("!"))
                            {

                            }
                            //If it's a test goal
                            else if (data.First().Contains("?"))
                            {

                            }
                            //It it's a belief
                            else
                            {
                                planList.Add(ParsePlan(data));
                            }
                            break;
                        default:
                            
                            data.Clear();
                            break;
                    }
                }
                else
                {
                    data.Add(line);
                }
            }
        }

        
        private Plan ParsePlan(List<string> data)
        {
            Plan p = null;
            #region TRIGGER
            char @operator = data.First().ElementAt(0);
            string s = data.First();
            string t = s.Substring(1, s.IndexOf("(") - 1);
            string aux = s.Split('(').ElementAt(1);
            aux = aux.Split(')').ElementAt(0);
            //s.Substring(s.IndexOf("("), (s.IndexOf(")")-s.IndexOf("(")));
            string[] parameters = aux.Split(',');
            data.RemoveAt(0);
            Trigger trigger = new Trigger(@operator, t, parameters);

            //TODO: Add the negative beliefs, the ones the agent doesn't belief

            #endregion
            #region CONTEXT
            //TODO: Chek if the context exists
            string context = data.First().Split('<').ElementAt(0);
            data.RemoveAt(0);
            #endregion

            #region PLANBODY
            List<Action> actions = new List<Action>();
            string name;
            string[] param;
            char symbol;

            foreach(string var in data)
            {
                switch (var.ElementAt(0))
                {
                    case '?':
                        break;
                    case '!':
                        break;
                    case '.':
                        symbol = var.ElementAt(var.Length - 1);
                        name = var.Split('(').ElementAt(0);
                        name = name.Split('.').ElementAt(1);
                        string auxi = var.Split('(').ElementAt(1);
                        auxi = auxi.Split(')').ElementAt(0);
                        param = auxi.Split(',');
                        actions.Add(new Action(name, param, symbol));
                        break;
                }
            }

            #endregion
            p = new Plan(trigger, context, new PlanBody(actions));
            return p;
        }
    }
}
