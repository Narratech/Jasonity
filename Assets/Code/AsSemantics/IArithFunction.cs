using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;

public interface IArithFunction
{
    // Returns the name of the function
    string GetName();

    // Evaluates/computes the function based on the arguments
    double Evaluate(Reasoner r, ITerm[] args);

    // Returns true if "a" is a good number of arguments for the function
    bool CheckArity(int a);

    // Returns true if the arguments of the function can be unground
    bool AllowUngroundTerms();
}
