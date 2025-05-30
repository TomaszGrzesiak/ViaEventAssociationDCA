using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Tools.OperationResult.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events.Entities
{
    public sealed class Invitation : Entity<InvitationId>
    {
        public GuestId GuestId { get; private set; }
        public EventId EventId { get; private set; }
        public InvitationStatus Status { get; private set; }

        private Invitation(InvitationId id, GuestId guestId, EventId eventId, InvitationStatus status) : base(id)
        {
            GuestId = guestId;
            EventId = eventId;
            Status = status;
        }

        public static Result<Invitation> Create(GuestId guestId, EventId eventId)
        {
            var invitation = new Invitation(InvitationId.CreateUnique(), guestId, eventId, InvitationStatus.Pending);
            return Result<Invitation>.Success(invitation);
        }

        public Result Approve()
        {
            if (Status.Equals(InvitationStatus.Approved))
                return Result.Failure(Error.InvitationAlreadyApproved);
            Status = InvitationStatus.Approved;
            return Result.Success();
        }

        public Result Reject()
        {
            if (Status.Equals(InvitationStatus.Rejected))
                return Result.Failure(Error.InvitationAlreadyRejected);

            Status = InvitationStatus.Rejected;
            return Result.Success();
        }
    }
}