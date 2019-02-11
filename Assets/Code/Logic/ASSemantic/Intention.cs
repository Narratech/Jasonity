using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Represents and Intention (a stack of IntendedMeans).
 *
 * The comparable sorts the intentions based on the atomic property:
 * atomic intentions comes first.
 *
 */
namespace Logica.ASSemantic
{
    [Serializable]
    public class Intention : IComparable<Intention>
    {
        public int CompareTo(Intention other)
        {
            throw new NotImplementedException();
        }
    }
}
