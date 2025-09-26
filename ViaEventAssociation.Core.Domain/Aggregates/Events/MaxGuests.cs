using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public class MaxGuests : ValueObject
{
    public int Value { get; }

    private MaxGuests(int value)
    {
        Value = value;
    }

    public static Result<MaxGuests> Create(int maxGuests)
    {
        return Validate(maxGuests);
    }

    private static Result<MaxGuests> Validate(int maxGuests)
    {
        return maxGuests switch
        {
            < 5 => Result<MaxGuests>.Failure(Error.GuestsMaxNumberTooSmall),
            > 50 => Result<MaxGuests>.Failure(Error.GuestsMaxNumberTooGreat),
            _ => Result<MaxGuests>.Success(new MaxGuests(maxGuests))
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        // if returning more than 1 value, then the receiver parses them one by one like in foreach,
        // instead of getting a full array of an object at a time.
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static MaxGuests Default()
    {
        return new MaxGuests(5);
    }
}