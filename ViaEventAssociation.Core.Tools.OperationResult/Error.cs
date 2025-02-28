namespace ViaEventAssociation.Core.Tools.OperationResult;

public class Error
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Code { get; }
    public string Message { get; }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }
    
}