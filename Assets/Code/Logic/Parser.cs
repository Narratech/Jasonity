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
        private List<Belief> beliefs;
        private List<Objective> objectives;
        private List<Plan> plans;

        public Parser()
        {
            this.beliefs = new List<Belief>();
            this.objectives = new List<Objective>();
            this.plans = new List<Plan>();
        }

        public void Parsing(string location/*Recibir la localización del fichero por parámetro??*/)
        {
            //string file = "@" + location;
            string aux, belief, subject, objective, @object, action, actionFocus;
            foreach(string line in File.ReadLines(@"E:\UnityProjects\
                Jasonity-master\Assets\Code\Logic\TextFiles\Bob.txt"))
            {
                switch (line[0])
                {
                    case '!':
                        //[-]say(hello)
                        aux = line.Remove(line.IndexOf('!'), line.IndexOf('!') + 1);
                        //[say](hello)
                        objective = aux.Substring(0, aux.IndexOf('('));
                        Console.WriteLine(objective);
                        //[---](hello)
                        @object = aux.Remove(0, aux.IndexOf('(') + 1);
                        Console.WriteLine(@object);
                        //[hello])
                        @object = @object.Substring(0, @object.IndexOf(')'));
                        Console.WriteLine(@object);
                        //say     //hello
                        objectives.Add(new Objective(objective, @object));
                        break;

                    case '+':
                        /*OBJETIVO DEL PLAN*/
                        //[--]say(X):happy(bob)<-print(X)
                        aux = line.Remove(line.IndexOf('+'), line.IndexOf('!') + 1);
                        //[say](X):happy(bob)<-print(X)
                        objective = aux.Substring(0, aux.IndexOf('('));
                        //[---](X):happy(bob)<-print(X)
                        @object = aux.Remove(0, aux.IndexOf('(') + 1);
                        //[X]):happy(bob)<-print(X)
                        @object = @object.Substring(0, @object.IndexOf(')'));
                        //say       //X
                        Objective o = new Objective(objective, @object);

                        /*CONDICIÓN DEL PLAN*/
                        //[--]happy(bob)<-print(X)
                        aux = aux.Remove(0, aux.IndexOf(':') + 1);
                        //[happy](bob)<-print(X)
                        belief = aux.Substring(0, aux.IndexOf('('));
                        //[------]bob)<-print(X)
                        subject = aux.Remove(0, aux.IndexOf('(') + 1);
                        //[bob])<-print(X)
                        subject = subject.Substring(0, subject.IndexOf(')'));
                        //happy   //bob
                        Belief b = new Belief(belief, subject);

                        /*FUNCIÓN DEL PLAN*/
                        //[------]print(X)
                        aux = aux.Remove(0, aux.IndexOf('-') + 1);
                        //[print](X)
                        action = aux.Substring(0, aux.IndexOf('('));
                        //[------]X)
                        actionFocus = aux.Remove(0, aux.IndexOf('(') + 1);
                        //[X])
                        actionFocus = actionFocus.Substring(0, actionFocus.IndexOf(')'));
                        plans.Add(new Plan(o, b, action, actionFocus));
                        break;

                    default:
                        //[happy](bob)
                        belief = line.Substring(0, line.IndexOf('('));
                        //[------]bob)
                        subject = line.Remove(0, line.IndexOf('(') + 1);
                        //[bob])
                        subject = subject.Substring(0, subject.IndexOf(')'));
                        //happy   //bob
                        beliefs.Add(new Belief(belief, subject));
                        break;
                }

            }
        }

        public List<Belief> Beliefs { get => this.beliefs; }

        public List<Objective> Objectives { get => this.objectives; }

        public List<Plan> Plans { get => this.plans; }
    }
}
