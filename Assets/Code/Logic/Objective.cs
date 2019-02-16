using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public class Objective : Term
    {
        /*This is the object of the objective*/
        private ObjectOfTheObjective @object;

        /*Constructor
            name: string
            @object: string
        */
        public Objective(string name, string @object) : base(name)
        {
            this.@object = new ObjectOfTheObjective(@object);
        }

        public override bool IsObjective()
        {
            return true;
        }

        /*Get for the object of the objective*/
        public ObjectOfTheObjective Object { get => this.@object; }
    }
}
