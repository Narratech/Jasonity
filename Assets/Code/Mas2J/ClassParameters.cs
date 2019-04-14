using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.AsSyntax;

namespace Assets.Code.Mas2J
{
    public class ClassParameters
    {
        private Structure structure;

        public ClassParameters(Structure structure)
        {
            this.structure = structure;
        }

        internal object GetParametersArray()
        {
            throw new NotImplementedException();
        }
    }
}
