using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;

public abstract class DefaultArithFunction : IArithFunction
{
    public string GetName() => GetType().Name; // Not sure if this is right

    public bool AllowUngroundTerms() => false;

    public bool CheckArity(int a) => true;

    public double Evaluate(Reasoner r, ITerm[] args) => 0;

    public override string ToString() => "function "+GetName();
}
