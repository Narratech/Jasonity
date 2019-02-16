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
        /*List with the agent's beliefs*/
        private List<Belief> beliefs;
        /*List with the agent's objectives*/
        private List<Objective> objectives;
        /*Lista with the agent's plans*/
        private List<Plan> plans;

        /*Constructor*/
        public Parser()
        {
            /*Lists inicialization*/
            this.beliefs = new List<Belief>();
            this.objectives = new List<Objective>();
            this.plans = new List<Plan>();
        }

        /*Parser for the text file*/
        public void Parsing(string location/*Recive the text file location through parameter??*/)
        {
            //string file = "@" + location;
            /*Auxiliar variables*/
            string aux, belief, subject, objective, @object, action, actionFocus;
            /*Loop for check the text file*/
            foreach(string line in File.ReadLines(@"E:\UnityProjects\
                Jasonity-master\Assets\Code\Logic\TextFiles\Bob.txt"))
            {
                switch (line[0])
                {
                    /*If the line start with a ! then is an objective*/
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

                    /*If the line start with a + then is a plan*/
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
                                          //say(X)  //happy(bob)    //print         //X
                        plans.Add(new Plan(  o,           b,        action,    actionFocus));
                        break;

                    /*If the line doesn`t start with a ! or a + then is a belief*/
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
        /*Get the list of beleifs*/
        public List<Belief> Beliefs { get => this.beliefs; }
        /*Get the list of objectives*/
        public List<Objective> Objectives { get => this.objectives; }
        /*Get the list of plans*/
        public List<Plan> Plans { get => this.plans; }
    }
}
