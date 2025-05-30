using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Common.Bases;

// Thanks to <T> we can make InvitationId and other Ids in just 1 Line of code.
// Otherwise, they could be used interchangeably - which is wrong conceptually.

public abstract class Id<T>(Guid value) : ValueObject where T : Id<T>
{
    public Guid Value { get; } = value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    protected static Result<Guid> CanParseGuid(string id)
    {
        bool canBeParsed = Guid.TryParse(id, out var guid);
        return canBeParsed
            ? Result<Guid>.Success(guid)
            : Result<Guid>.Failure(Error.UnParsableGuid);
    }

    public static Result<T> FromString(string id)
    {
        var result = CanParseGuid(id);
        return result.IsFailure
            ? Result<T>.Failure(result.Errors.ToArray())
            : Result<T>.Success((T)Activator.CreateInstance(typeof(T), result.Payload!)!);
    }

    // works like this, but for T type: f.x. InvitationId Generate() => new(Guid.NewGuid());
    public static T Generate() => (T)Activator.CreateInstance(typeof(T), Guid.NewGuid())!;
    public static T FromGuid(Guid id) => (T)Activator.CreateInstance(typeof(T), id)!;
}