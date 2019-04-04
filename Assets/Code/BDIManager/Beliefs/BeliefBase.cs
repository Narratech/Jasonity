// Interface for Belief Bases
// Holds all of the agents' beliefs
// Allows the user to modify the Belief Base and check for relevant data
using Assets.Code.Agent;
using Assets.Code.Logic;
using System.Collections.Generic;

namespace BDIManager.Beliefs
{
    interface BeliefBase
    {

        // Removes all beliefs from BB
        void Clear();

        // Adds a belief at the end of the BB
        // Returns true if success
        bool Add(Literal l);

        // Adds a belief at the index position of the BB
        bool Add(int index, Literal l);

        // Returns the literal
        Literal Contains(Literal l);

        // Returns the number of beliefs in BB
        int Size();

        // Removes a literal from the BB
        // Returns true if success
        bool Remove(Literal l);
    }
}