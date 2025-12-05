using ViaEventAssociation.Core.Domain.Contracts;

namespace ViaEventAssociation.Presentation.WebAPI;

public sealed class SystemTime : ISystemTime
{
    public DateTime Now() => DateTime.UtcNow;
}