using System.Globalization;
using System.Reflection;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Common.Bases;

// Thanks to <T> we can make InvitationId and other Ids in just 1 Line of code.
// Otherwise, they could be used interchangeably - which is wrong conceptually.
public abstract class Id<T>(Guid value) : ValueObject where T : Id<T>
{
    public Guid Value { get; } = value;
    protected Id() : this(Guid.Empty) {}
    public override string ToString() => Value.ToString();
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    private static Result<Guid> CanParseGuid(string id)
    {
        bool canBeParsed = Guid.TryParse(id, out var guid);
        return canBeParsed
            ? Result<Guid>.Success(guid)
            : Result<Guid>.Failure(Error.UnParsableGuid);
    }

    // helper that can call protected/private ctors with the (Guid) signature
    private static T CreateWith(Guid id) =>
        (T)Activator.CreateInstance(
            typeof(T),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { id },
            culture: CultureInfo.InvariantCulture
        )!;

    public static Result<T> FromString(string id)
    {
        var result = CanParseGuid(id);
        return result.IsFailure
            ? Result<T>.Failure(result.Errors.ToArray())
            : Result<T>.Success(CreateWith(result.Payload!));
    }

    public static T CreateUnique() => CreateWith(Guid.NewGuid());
    public static T FromGuid(Guid id) => CreateWith(id);
}