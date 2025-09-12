namespace ViaEventAssociation.Core.Tools.OperationResult;

public class Error
{
    public int Code { get; }
    public string Message { get; }

    private static readonly HashSet<int> UsedCodes = new();

    private Error(int code, string message)
    {
        Code = code;
        Message = message;
    }

    private static Error Create(int code, string message) =>
        UsedCodes.Add(code)
            ? new Error(code, message)
            : throw new InvalidOperationException($"-- == :: Duplicate ERROR code detected in Error.cs :: == --: {code}");

    // for testing purposes
    public static readonly Error TestError1 = Create(1, "Error 1");
    public static readonly Error TestError2 = Create(2, "Error 2");

    // single or doublet errors (100-139)
    public static readonly Error UnParsableGuid = Create(100, "Could not parse the given Guid.");
    public static readonly Error GuestsMaxNumberTooSmall = Create(101, "Too small number of guests. Must be at least 5.");
    public static readonly Error UnknownError = Create(911, "Unknown Error");

    public static readonly Error EventTitleMustBeBetween3And75Characters =
        Create(102, "Event title must be between 3 and 75 characters long, excluding white space.");

    public static readonly Error EventDescriptionCannotExceed250Characters = Create(105, "Description cannot be more than 250 characters.");
    public static readonly Error InvalidNameFormat = Create(106, "Invalid name(s). Both First and Last name must be 2–25 letters (a–z only).");
    public static readonly Error InvalidInvitationStatus = Create(107, "Invalid invitation status.");
    public static readonly Error InvalidEventStatus = Create(108, "Invalid event status.");
    public static readonly Error InvalidEventVisibility = Create(109, "Invalid event visibility.");
    public static readonly Error GuestsMaxNumberTooGreat = Create(110, "Too large number of guests. Must be max 50.");

    public static readonly Error InvalidProfilePictureUrlEmpty =
        Create(111, "Profile picture URL is invalid or empty. Please provide a valid URL starting with http or https. ");

    // email errors (codes 140 - 149)
    public static readonly Error EmailRequired = Create(140, "Email is required.");
    public static readonly Error EmailMustEndWithViaDomain = Create(141, "Email must end with '@via.dk'.");

    public static readonly Error EmailInvalidFormat =
        Create(142, "Email must be in the format <text1>@<text2>.<text3>. The part before @ must be either 3–4 letters or 6 digits.");


    // time range errors (150-169)
    public static readonly Error EventTimeRangeMissing = Create(150, "Either start or end time is missing.");
    public static readonly Error EventTimeStartAfterEndTime = Create(151, "Start time cannot be after end time.");
    public static readonly Error EventTimeStartDateAfterEndDate = Create(152, "Start date cannot be after end date.");
    public static readonly Error EventTimeDurationTooShort = Create(153, "Time duration must be at least 1 hour.");
    public static readonly Error EventTimeDurationTooLong = Create(154, "Time duration cannot be more than 10 hours.");
    public static readonly Error EventTimeInvalidEndTimeWindow = Create(155, "End time must be before 23:59 on same day or before 01:00 on the next day.");
    public static readonly Error EventTimeStartMustBeAfter8Am = Create(156, "Start time must be after 08:00.");
    public static readonly Error EventTimeCannotStartInPast = Create(157, "Start must not be in the past.");
    public static readonly Error EventTimeCannotSpan01To08 = Create(158, "Event cannot span between 01:00 and 08:00.");

    // invitation errors (170–179)
    public static readonly Error InvitationAlreadyApproved = Create(170, "Invitation is already approved.");
    public static readonly Error InvitationAlreadyRejected = Create(171, "Invitation is already rejected.");
    public static readonly Error DeclineImpossibleOnCancelledEvent = Create(172, "Invitations to cancelled events cannot be declined.");

    // Event-related errors (codes 180-249)
    public static readonly Error GuestAlreadyInvited = Create(180, "This guest is already invited to the event.");
    public static readonly Error NoMoreRoom = Create(181, "There is no more room for guests in the event.");
    public static readonly Error EventAlreadyActive = Create(182, "The event is already active.");
    public static readonly Error EventAlreadyCancelled = Create(183, "The event is already cancelled and cannot be modified any further.");
    public static readonly Error EventIsNotPublic = Create(184, "Only public events allow guests to join freely.");
    public static readonly Error GuestAlreadyJoined = Create(185, "This guest has already joined the event.");
    public static readonly Error UpdateMaxGuestsImpossible = Create(186, "Update max guests is impossible.");
    public static readonly Error UpdateVisibilityImpossible = Create(187, "Update event visibility is impossible.");
    public static readonly Error ActivateFailure = Create(188, "Could not activate the event.");
    public static readonly Error ActiveOrCanceledEventCannotBeModified = Create(189, "Active or canceled event cannot be modified.");
    public static readonly Error DecreaseMaxGuestsImpossible = Create(190, "Decrease max guests is impossible.");
    public static readonly Error MaxGuestAboveLocationCapacity = Create(191, "You cannot have more people at an event than there is room for.");
    public static readonly Error EventTitleCannotBeDefaultOrEmpty = Create(192, "Event title cannot remain default or empty. Please change it first.");
    public static readonly Error EventDescriptionCannotBeDefault = Create(194, "Event description cannot remain default or not set. Please change it first.");
    public static readonly Error EventVisibilityMustBeSet = Create(195, "Event visibility is not set. Please set it first.");
    public static readonly Error CannotReadyPastEvent = Create(196, "An event in the past cannot be made ready.");
    public static readonly Error OnlyActiveEventsCanBeJoined = Create(197, "Only active events can be joined.");
    public static readonly Error TooLate = Create(198, "Too late. Next time be quicker to join the event!");
    public static readonly Error CanOnlyInviteToReadyOrActiveEvent = Create(199, "Guests can only be invited to the event, when the event is ready or active.");

    // Guest-related error (codes  250-299)
    public static readonly Error EmailAlreadyRegistered = Create(250, "The email is already registered.");
    public static readonly Error InvitationNotFound = Create(251, "Invitation not found.");
    public static readonly Error CancelledEventsCannotBeJoined = Create(252, "Cancelled events cannot be joined.");
    public static readonly Error JoinUnstartedEventImpossible = Create(253, "Join unstarted event impossible.");

    // Not-grouped errors (codes 300-349)
    public static readonly Error InvalidGuestId = Create(300, "GuestId must be greater than 0.");
    public static readonly Error InvalidEventId = Create(301, "EventId must be greater than 0.");
    public static readonly Error CommandCannotBeNull = Create(302, "Command cannot be null");
    public static readonly Error GuidIsRequired = Create(303, "Guid is required.");

    // Commands and handlers (codes 350 - 400)
    public static readonly Error CantFindEventWithThisId = Create(304, "The system can't find any event with this id.");
    public static readonly Error TimeNotParsable = Create(305, "Given Start or End Time is not parsable from string to DateTime format.");
}