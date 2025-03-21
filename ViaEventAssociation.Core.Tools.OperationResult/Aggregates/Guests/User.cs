namespace ViaEventAssociation.Core.Tools.OperationResult;

public class User
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Username { get; set; }
    public string Password { get; set; }
}