using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public sealed class EventDescription : ValueObject
{
    public string Value { get; }

    private EventDescription(string value)
    {
        Value = value;
    }

    public static Result<EventDescription> Create(string? input)
    {
        if (input is null)
        {
            return Result<EventDescription>.Failure(Error.EventDescriptionCannotBeNull);
        }

        if (input.Length > 250)
        {
            return Result<EventDescription>.Failure(Error.EventDescriptionCannotExceed250Characters);
        }

        return Result<EventDescription>.Success(new EventDescription(input));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}