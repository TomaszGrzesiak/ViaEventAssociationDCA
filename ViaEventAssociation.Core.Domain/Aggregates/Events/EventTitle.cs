using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public class EventTitle : ValueObject
{
    public string? Value { get; }

    private EventTitle() {}  // for EF only
    private EventTitle(string value)
    {
        Value = value;
    }

    public static Result<EventTitle> Create(string? title)
    {
        return Validate(title);
    }

    private static Result<EventTitle> Validate(string? title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 75 || title.Length < 3)
            return Result<EventTitle>.Failure(Error.EventTitleMustBeBetween3And75Characters);

        return Result<EventTitle>.Success(new EventTitle(title.Trim()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static EventTitle Default()
    {
        return new EventTitle("Working title.");
    }
}