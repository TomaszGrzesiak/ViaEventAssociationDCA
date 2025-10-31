using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events.Entities
{
    public class Invitation : Entity<InvitationId>
    {
        // for EF only
        private Invitation() { }
        public GuestId GuestId { get; private set; }

        public InvitationStatus Status { get; private set; }

        private Invitation(InvitationId id, GuestId guestId, InvitationStatus status) : base(id)
        {
            GuestId = guestId;
            Status = status;
        }

        public static Invitation Create(GuestId guestId)
        {
            return new Invitation(InvitationId.CreateUnique(), guestId, InvitationStatus.Pending);
        }

        public Result Approve()
        {
            if (Status.Equals(InvitationStatus.Accepted))
                return Result.Failure(Error.InvitationAlreadyApproved);
            Status = InvitationStatus.Accepted;
            return Result.Success();
        }

        public Result Reject()
        {
            if (Status.Equals(InvitationStatus.Declined))
                return Result.Failure(Error.InvitationAlreadyRejected);

            Status = InvitationStatus.Declined;
            return Result.Success();
        }
    }
}