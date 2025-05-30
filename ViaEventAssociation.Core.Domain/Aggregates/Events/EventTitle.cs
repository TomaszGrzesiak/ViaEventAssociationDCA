using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Events.ValueObjects;

public class EventTitle : ValueObject
{
    public string Value { get; }

    private EventTitle(string value)
    {
        Value = value;
    }

    public static Result<EventTitle> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<EventTitle>.Failure(Error.EventTitleCannotBeEmpty);

        if (title.Length > 100)
            return Result<EventTitle>.Failure(Error.EventTitleCannotExceed100Characters);

        return Result<EventTitle>.Success(new EventTitle(title.Trim()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}