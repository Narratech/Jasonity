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
            foreach(string line in File.ReadLines(@"E:\UnityProjects\
                Jasonity-master\Assets\Code\Logic\TextFiles\Bob.txt"))
            {
                switch (line[0])
                {
                    case '!':
                        //![decir](hello)
                        string objective = line.Substring(1, line.IndexOf('(')-1);
                        Console.WriteLine(objective);
                        //[------](hello)
                        line.Remove(0, line.IndexOf('('));
                        Console.WriteLine(line);
                        //[hello])
                        string @object = line.Substring(0, line.IndexOf(')') - 1);
                                                    //say     //hello
                        objectives.Add(new Objective(objective, @object));
                        break;

                    case '+':
                        //Pendiente (Misma idea)
                        break;

                    default:
                        //[happy](bob)
                        string belief = line.Substring(0, line.IndexOf('('));
                        Console.WriteLine(belief);
                        //[------]bob)
                        line.Remove(0, line.IndexOf('('));
                        Console.WriteLine(line);
                        //[bob])
                        string subject = line.Substring(0, line.IndexOf(')')-1);
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
