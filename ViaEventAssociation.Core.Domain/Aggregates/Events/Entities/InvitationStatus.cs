namespace ViaEventAssociation.Core.Domain.Aggregates.Events.Entities
{
    using ViaEventAssociation.Core.Domain.Common.Bases;
    using ViaEventAssociation.Core.Tools.OperationResult;

    public class InvitationStatus : Enumeration
    {
        public static readonly InvitationStatus Pending = new(1, "Pending");
        public static readonly InvitationStatus Accepted = new(2, "Accepted");
        public static readonly InvitationStatus Rejected = new(3, "Rejected");

        private InvitationStatus(int id, string name) : base(id, name)
        {
        }

        // prolly needed when read from DB, APIs, UI as int
        public static Result<InvitationStatus> FromId(int id) =>
            GetAll<InvitationStatus>().FirstOrDefault(s => s.Id == id) is { } match
                ? Result<InvitationStatus>.Success(match)
                : Result<InvitationStatus>.Failure(Error.InvalidInvitationStatus);

        // prolly needed when read from DB, APIs, UI as string
        public static Result<InvitationStatus> FromName(string? name) =>
            GetAll<InvitationStatus>().FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)) is { } match
                ? Result<InvitationStatus>.Success(match)
                : Result<InvitationStatus>.Failure(Error.InvalidInvitationStatus);
    }
}