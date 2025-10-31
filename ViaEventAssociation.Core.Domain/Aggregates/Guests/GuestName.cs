using System.Text.RegularExpressions;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public class GuestName : ValueObject
{
    public string? Value { get; private set; }

    private GuestName() {}  // for EF only
    private GuestName(string name)
    {
        Value = name;
    }

    public static Result<GuestName> Create(string? name)
    {
        return Validate(name);
    }

    private static Result<GuestName> Validate(string? name)
    {
        if (name is not null && name.Length is >= 2 and <= 25 && Regex.IsMatch(name, @"^[a-zA-Z]+$"))
            return Result<GuestName>.Success(new GuestName(FormatName(name)));
        return Result<GuestName>.Failure(Error.InvalidNameFormat);
    }

    private static string FormatName(string name)
    {
        return char.ToUpper(name[0]) + name[1..].ToLower();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value ?? "";
    }

    public override string ToString() => Value ?? "";
}