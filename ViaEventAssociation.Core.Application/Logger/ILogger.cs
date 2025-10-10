namespace ViaEventAssociation.Core.Application.Logger;

public interface ILogger
{
    void Info(string message);
    void Error(string message, Exception ex);
}