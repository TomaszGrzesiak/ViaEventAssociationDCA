using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events
{
    public class EventStatus : Enumeration
    {
        public static readonly EventStatus Draft = new(1, "Draft");
        public static readonly EventStatus Active = new(2, "Active");
        public static readonly EventStatus Cancelled = new(3, "Cancelled");
        public static readonly EventStatus Ready = new(4, "Ready");

        private EventStatus() {}
        private EventStatus(int id, string name) : base(id, name)
        {
        }


        // prolly needed when read from DB, APIs, UI as int
        public static Result<EventStatus> FromId(int id) =>
            GetAll<EventStatus>().FirstOrDefault(s => s.Id == id) is { } match
                ? Result<EventStatus>.Success(match)
                : Result<EventStatus>.Failure(Error.InvalidEventStatus);

        // prolly needed when read from DB, APIs, UI as string
        public static Result<EventStatus> FromName(string? name) =>
            GetAll<EventStatus>().FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)) is { } match
                ? Result<EventStatus>.Success(match)
                : Result<EventStatus>.Failure(Error.InvalidEventStatus);
    }
}