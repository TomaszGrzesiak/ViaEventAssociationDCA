namespace ViaEventAssociation.Core.Tools.OperationResult.Common.Bases;

public class EventId
{
    public Guid Value = new Guid();

    public EventId(Guid value)
    {
        Value = value;
    }
}