namespace ViaEventAssociation.Core.Tools.OperationResult.Aggregates.Locations;

public class Location
{
    public Guid Id { get; set; } = new Guid();
    public string Type { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }

    public Location(string type, string name, int capacity)
    {
        Type = type;
        Name = name;
        Capacity = capacity;
    }
}