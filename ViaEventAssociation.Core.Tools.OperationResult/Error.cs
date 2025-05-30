namespace ViaEventAssociation.Core.Tools.OperationResult;

public sealed class Error
{
    private static readonly HashSet<int> UsedCodes = new();

    // custom errors (120 - 150)
    public static readonly Error UnParsableGuid = Create(120, "Could not parse the given Guid.");
    public static readonly Error GuestsMaxNumberTooSmall = Create(121, "Too small number of guests. Must be at least 5.");
    public static readonly Error EventTitleCannotBeEmpty = Create(122, "Event title cannot be empty.");
    public static readonly Error EventTitleCannotExceed100Characters = Create(123, "Event title cannot exceed 100 characters.");

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