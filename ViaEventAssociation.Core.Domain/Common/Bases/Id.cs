using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Common.Bases;

// The <T> serves for type safety, e.g. EventId != GuestId.
// Otherwise, they could be used interchangeably - which is wrong conceptually.
public abstract class Id<T>(Guid value) : ValueObject
{
    public Guid Value { get; } = value;

    public override string ToString()
    {
        return Value.ToString();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    // you never know...
    protected static Result<Guid> CanParseGuid(string id)
    {
        var canBeParsed = Guid.TryParse(id, out var guid);
        return canBeParsed ? Result<Guid>.Success(guid) : Result<Guid>.Failure(ResultError.UnParsableGuid);
    }
}