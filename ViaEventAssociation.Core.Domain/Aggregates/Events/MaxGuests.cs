using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Invitations.ValueObjects;

public class MaxGuests : ValueObject
{
    public int Value { get; }

    private MaxGuests(int value)
    {
        Value = value;
    }

    public static Result<MaxGuests> Create(int maxGuests)
    {
        return maxGuests < 5
            ? Result<MaxGuests>.Failure(Error.GuestsMaxNumberTooSmall)
            : Result<MaxGuests>.Success(new MaxGuests(maxGuests));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        // if returning more than 1 value, then the receiver parses them one by one like in foreach,
        // instead of getting a full array of an object at a time.
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}