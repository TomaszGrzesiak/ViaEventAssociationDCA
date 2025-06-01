using System.Text.RegularExpressions;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public sealed class GuestName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    private GuestName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<GuestName> Create(string? firstName, string? lastName)
    {
        if (!IsValidName(firstName) || !IsValidName(lastName))
            return Result<GuestName>.Failure(Error.InvalidFirstOrLastName);

        string formattedFirst = FormatName(firstName!);
        string formattedLast = FormatName(lastName!);

        return Result<GuestName>.Success(new GuestName(formattedFirst, formattedLast));
    }

    private static bool IsValidName(string? name)
    {
        return name is not null &&
               name.Length is >= 2 and <= 25 &&
               Regex.IsMatch(name, @"^[a-zA-Z]+$");
    }

    private static string FormatName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToUpper(name[0]) + name[1..].ToLower();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString() => $"{FirstName} {LastName}";
}