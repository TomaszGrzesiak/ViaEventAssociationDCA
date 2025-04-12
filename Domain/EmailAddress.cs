using ViaEventAssociation.Core.Tools.OperationResult;


namespace Domain;

public class EmailAddress
{
    public string Value { get; }
    
    // private constructor - to be utilized by the static methods below
    private EmailAddress(string emailAddress)
        => Value = emailAddress;
    
    public static Result<EmailAddress> Create (string emailAddress)
    {
        Result<string> validation = Validate(emailAddress);
        
        // if validation sees an error in the typed emailAddress, the user gets Result with an error message:
        if (validation.IsFailure) return Result<EmailAddress>.Failure(validation.ErrorMessages);
        
        // if validation is OK then the user gets Result with Success and with a payload of an EmailAddress object:
        EmailAddress value = new EmailAddress(validation.Payload!);
        return Result<EmailAddress>.Success(value);
    }
    private static Result<string> Validate(string emailAddress)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(emailAddress))
            return Result<string>.Failure("Email is required.");

        // trimming empty spaces and converting to lower case @ID:10 - registering a new Guest account: "And the email is in all lower-case"
        var email = emailAddress.Trim().ToLowerInvariant();

        // Must end with @via.dk
        if (!email.EndsWith("@via.dk"))
            errors.Add("Email must end with '@via.dk'.");

        // Must match format <text1>@<text2>.<text3>
        var parts = email.Split('@');
        if (parts.Length != 2 || !parts[1].Contains('.'))
            errors.Add("Email must be in the format <text1>@<text2>.<text3>");

        var text1 = parts[0];

        if (text1.Length < 3 || text1.Length > 6)
            errors.Add("The part before @ must be between 3 and 6 characters long.");

        bool is3Or4Letters = (text1.Length == 3 || text1.Length == 4) && text1.All(char.IsLetter);
        bool is6Digits = text1.Length == 6 && text1.All(char.IsDigit);

        if (!is3Or4Letters && !is6Digits) // If the text1 is not 3-4 letters and also not 6 digits then it's invalid
            errors.Add("The part before @ must be either 3–4 letters or 6 digits.");

        if (errors.Count > 0)
            return Result<string>.Failure(errors);

        return Result<string>.Success(email);
    }
}
