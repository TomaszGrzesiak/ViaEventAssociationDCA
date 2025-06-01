using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events
{
    public sealed class EventVisibility : Enumeration
    {
        public static readonly EventVisibility Public = new(1, "Public");
        public static readonly EventVisibility Private = new(2, "Private");

        private EventVisibility(int id, string name) : base(id, name)
        {
        }

        // prolly needed when read from DB, APIs, UI as int
        public static Result<EventVisibility> FromId(int id) =>
            GetAll<EventVisibility>().FirstOrDefault(v => v.Id == id) is { } match
                ? Result<EventVisibility>.Success(match)
                : Result<EventVisibility>.Failure(Error.InvalidEventVisibility);

        // prolly needed when read from DB, APIs, UI as string
        public static Result<EventVisibility> FromName(string? name) =>
            GetAll<EventVisibility>().FirstOrDefault(v => string.Equals(v.Name, name, StringComparison.OrdinalIgnoreCase)) is { } match
                ? Result<EventVisibility>.Success(match)
                : Result<EventVisibility>.Failure(Error.InvalidEventVisibility);
    }
}