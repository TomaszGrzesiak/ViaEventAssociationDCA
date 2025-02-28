namespace ViaEventAssociation.Core.Tools.OperationResult;

public class Calculator
{
    public int A { get; }
    public int B { get; }

    public Calculator(int a, int b)
    {
        A = a;
        B = b;
    }

    public int Sum() => A + B;
}