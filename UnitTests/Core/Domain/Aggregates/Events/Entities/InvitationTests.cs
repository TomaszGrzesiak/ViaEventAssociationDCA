using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Tools.OperationResult.Common.Bases;

namespace UnitTests.Core.Domain.Aggregates.Events.Entities;

public class InvitationTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccessAndPendingStatus()
    {
        var guestId = GuestId.CreateUnique();
        var eventId = EventId.CreateUnique();

        var result = Invitation.Create(guestId, eventId);

        Assert.True(result.IsSuccess);
        Assert.Equal(guestId, result.Payload!.GuestId);
        Assert.Equal(eventId, result.Payload.EventId);
        Assert.Equal(InvitationStatus.Pending, result.Payload.Status);
    }

    [Fact]
    public void Approve_WhenPending_SetsStatusToApproved()
    {
        var invitation = Invitation.Create(GuestId.CreateUnique(), EventId.CreateUnique()).Payload;

        var result = invitation!.Approve();

        Assert.True(result.IsSuccess);
        Assert.Equal(InvitationStatus.Approved, invitation.Status);
    }

    [Fact]
    public void Approve_WhenAlreadyApproved_ReturnsFailure()
    {
        var invitation = Invitation.Create(GuestId.CreateUnique(), EventId.CreateUnique()).Payload;

        _ = invitation!.Approve();
        var result = invitation.Approve();

        Assert.True(result.IsFailure);
        Assert.Equal(Error.InvitationAlreadyApproved, result.Errors[0]);
    }

    [Fact]
    public void Reject_WhenPending_SetsStatusToRejected()
    {
        var invitation = Invitation.Create(GuestId.CreateUnique(), EventId.CreateUnique()).Payload;

        var result = invitation!.Reject();

        Assert.True(result.IsSuccess);
        Assert.Equal(InvitationStatus.Rejected, invitation.Status);
    }

    [Fact]
    public void Reject_WhenAlreadyRejected_ReturnsFailure()
    {
        var invitation = Invitation.Create(GuestId.CreateUnique(), EventId.CreateUnique()).Payload;
        _ = invitation!.Reject();

        var result = invitation.Reject();

        Assert.True(result.IsFailure);
        Assert.Equal(Error.InvitationAlreadyRejected, result.Errors[0]);
    }
}