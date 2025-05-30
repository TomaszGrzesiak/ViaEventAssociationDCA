namespace ViaEventAssociation.Core.Tools.OperationResult;

public sealed class Error
{
    private static readonly HashSet<int> UsedCodes = new();

    // custom errors (120 - 150)
    public static readonly Error UnParsableGuid = Create(120, "Could not parse the given Guid.");
    public static readonly Error GuestsMaxNumberTooSmall = Create(121, "Too small number of guests. Must be at least 5.");
    public static readonly Error EventTitleCannotBeEmpty = Create(122, "Event title cannot be empty.");
    public static readonly Error EventTitleCannotExceed100Characters = Create(123, "Event title cannot exceed 100 characters.");
    public static readonly Error EventDescriptionCannotBeNull = Create(124, "Description cannot be null.");
    public static readonly Error EventDescriptionCannotExceed250Characters = Create(125, "Description cannot be more than 250 characters.");

    // time range errors (150-160)
    public static readonly Error EventTimeRangeMissing = Create(150, "Time range is missing.");
    public static readonly Error EventTimeStartAfterEndTime = Create(151, "Start time cannot be after end time.");
    public static readonly Error EventTimeDurationTooShort = Create(152, "Time duration must be at least 1 hour.");
    public static readonly Error EventTimeDurationTooLong = Create(153, "Time duration cannot be more than 10 hours.");
    public static readonly Error EventTimeInvalidEndTimeWindow = Create(154, "End time must be before 23:59 on same day or before 01:00 on the next day.");
    public static readonly Error EventTimeInvalidStartTime = Create(155, "Start time must be after 08:00.");

    // email errors (codes 100 - 120)
    public static readonly Error EmailRequired = Create(100, "Email is required.");
    public static readonly Error EmailNotEndingWithViaDk = Create(101, "Email must end with '@via.dk'.");
    public static readonly Error EmailDoesNotFollowFormatText1AtText2DotText3 = Create(102, "Email must be in the format <text1>@<text2>.<text3>.");
    public static readonly Error EmailPartBeforeAtMustBeBetween3And6CharactersLong = Create(103, "The part before @ must be between 3 and 6 characters long.");
    public static readonly Error EmailPartBeforeAtMustBeEither3Or4LettersOr6Digits = Create(104, "The part before @ must be either 3–4 letters or 6 digits.");

    private Error(int code, string message)
    {
        Code = code;
        Message = message;
    }

    private static Error Create(int code, string message) =>
        UsedCodes.Add(code)
            ? new Error(code, message)
            : throw new InvalidOperationException($"Duplicate error code detected: {code}");


    public int Code { get; }
    public string Message { get; }

#if DEBUG
    public static Error CustomForUnitTestsOnly(int code, string message)
    {
        return new Error(code, message);
    }
#endif
}