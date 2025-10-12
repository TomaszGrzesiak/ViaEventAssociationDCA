using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public class EmailAddress : ValueObject
{
    public string Value { get; }

    // private constructor - to be used by the static methods below
    private EmailAddress(string emailAddress)
        => Value = emailAddress;

    public static Result<EmailAddress> Create(string? emailAddress)
    {
        return Validate(emailAddress);
    }

    private static Result<EmailAddress> Validate(string? rawEmail)
    {
        if (string.IsNullOrWhiteSpace(rawEmail))
            return Result<EmailAddress>.Failure(Error.EmailCannotBeEmpty);

        // trimming empty spaces and converting to lower case
        var email = rawEmail.Trim().ToLowerInvariant();

        var errors = new List<Error>();

        // Must end with @via.dk
        if (!email.EndsWith("@via.dk"))
            errors.Add(Error.EmailMustEndWithViaDomain);

        var parts = email.Split('@');

        // Must match format <text1>@<text2>.<text3>
        if (parts.Length != 2 || !parts[1].Contains('.'))
            errors.Add(Error.EmailInvalidFormat);

        var text1 = parts[0];

        if (text1.Length < 3 || text1.Length > 6)
            errors.Add(Error.EmailInvalidFormat);

        bool is3Or4Letters = (text1.Length == 3 || text1.Length == 4) && text1.All(char.IsLetter);
        bool is6Digits = text1.Length == 6 && text1.All(char.IsDigit);

        if (!is3Or4Letters && !is6Digits) // If the text1 is not 3-4 letters and also not 6 digits, then it's invalid
            errors.Add(Error.EmailInvalidFormat);

        if (errors.Count > 0)
            return Result<EmailAddress>.Failure(errors.ToArray());

        return Result<EmailAddress>.Success(new EmailAddress(email));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value ?? "";
    }

    public override string ToString() => Value ?? "";
}