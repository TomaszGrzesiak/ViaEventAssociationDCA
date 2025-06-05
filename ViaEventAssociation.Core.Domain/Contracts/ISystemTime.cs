namespace ViaEventAssociation.Core.Domain.Contracts;

public interface ISystemTime
{
    Task GetSystemTimeUtc();
}