// using UnitTests.Fakes;
// using UnitTests.Helpers;
// using ViaEventAssociation.Core.Domain.Aggregates.Events;
// using ViaEventAssociation.Core.Domain.Contracts;
// using ViaEventAssociation.Core.Tools.OperationResult;
//
// namespace UnitTests.Core.Domain.Aggregates.Guests.UseCasesTests;
//
// public class EventTestsId12
// {
//     private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));
//     DateTime today = new DateTime(FakeSystemTime.Now().Year, FakeSystemTime.Now().Month, FakeSystemTime.Now().Day, 0, 0, 0);
//
//     [Fact]
//     public void Id12_S1_RemovesParticipation_WhenGuestIsParticipating()
//     {
//         var guest = GuestFactory.Init().Build();
//         var veaEvent = EventFactory.Init()
//             .WithGuest(guest.Id)
//             .Build();
//
//         Assert.Contains(veaEvent.GuestList, g => g == guest.Id);
//
//         var result = veaEvent.CancelParticipation(guest.Id, FakeSystemTime);
//         Assert.True(result.IsSuccess);
//         Assert.DoesNotContain(veaEvent.GuestList, g => g == guest.Id);
//     }
//
//     [Fact]
//     public void Id12_S2_Noop_WhenGuestIsNotParticipating()
//     {
//         var guest = GuestFactory.Init().Build();
//         var veaEvent = EventFactory.Init()
//             .Build();
//
//         Assert.DoesNotContain(veaEvent.GuestList, g => g == guest.Id);
//         var result = veaEvent.CancelParticipation(guest.Id, FakeSystemTime);
//
//         Assert.True(result.IsSuccess);
//         Assert.DoesNotContain(veaEvent.GuestList, g => g == guest.Id);
//     }
//
//     [Fact]
//     public void Id12_F1_Failure_WhenEventStartedAlready()
//     {
//         var pastStart = EventTimeRange.Default(FakeSystemTime).StartTime.AddDays(-2);
//         var pastDateTime = EventTimeRange.Create(pastStart, pastStart.AddHours(2)).Payload!;
//
//         var guest = GuestFactory.Init().Build();
//         var veaEvent = EventFactory.Init()
//             .WithGuest(guest.Id)
//             .WithTimeRange(pastDateTime)
//             .Build();
//
//         var result = veaEvent.CancelParticipation(guest.Id, FakeSystemTime);
//
//         Assert.True(result.IsFailure);
//         Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
//     }
// }