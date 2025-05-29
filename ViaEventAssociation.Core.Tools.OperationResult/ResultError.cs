namespace ViaEventAssociation.Core.Tools.OperationResult;


public sealed class ResultError
{
    public int Code { get; }
    public string Message { get; }

    private ResultError(int code, string message)
    {
        Code = code;
        Message = message;
    }
    
    // email errors (codes 100 - 120)
    public static ResultError EmailRequired =
        new(100, "Email is required.");

    public static readonly ResultError EmailNotEndingWithViaDk =
        new(101, "Email must end with '@via.dk'.");

    public static ResultError EmailDoesNotFollowFormatText1AtText2DotText3 =
        new(102, "Email must be in the format <text1>@<text2>.<text3>.");
    public static ResultError EmailPartBeforeAtMustBeBetween3And6CharactersLong =
        new(103, "The part before @ must be between 3 and 6 characters long.");
    public static ResultError EmailPartBeforeAtMustBeEither3Or4LettersOr6Digits =
        new(104, "The part before @ must be either 3–4 letters or 6 digits.");

#if DEBUG
    public static ResultError CustomForUnitTestsOnly(int code, string message) => new(code, message);
#endif
}
