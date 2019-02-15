// Interface for Belief Bases
// Holds all of the agents' beliefs
// Allows the user to modify the Belief Base and check for relevant data
using Jason.Logic.AsSyntax;
using Logica.ASSemantic;

namespace BDIManager.Beliefs
{
    interface BeliefBase
    {
        // Called before the execution with the agent that uses this BB
        void Init(Agent ag, string[] args);

        // Called before the end of execution
        void Stop();

        // Removes all beliefs from BB
        void Clear();

        // Adds a belief at the end of the BB
        // Returns true if success
        bool Add(Literal l);

        // Returns the literal
        Literal Contains(Literal l);

        // Returns the number of beliefs in BB
        int Size();

        // Removes a literal from the BB
        // Returns true if success
        bool Remove(Literal l);
    }
}