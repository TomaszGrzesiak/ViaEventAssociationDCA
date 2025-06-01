namespace ViaEventAssociation.Core.Domain.Common.Bases;

// "Session 2 - DDD.pptx" slide 70. Copy - pasted from the slide + some extras.

// Assignment 4:
// “You must make a generic Entity base class, where the generic type parameter will be the type of the ID,
// given in the sub-class, e.g. public class VeaEvent : Entity<EventId>.”
public abstract class Entity<TId>
    where TId : ValueObject // ValueObject, so they can compare in Equals()
{
    protected Entity()
    {
    }

    protected Entity(TId id)
    {
        Id = id;
    }

    // TId is a generic type of the ID, given in the sub-class, e.g. public class VeaEvent : Entity<EventId>.
    // F.x. Despite both EventId and GuestId extend Id, they won't be equal or can't be assigned as ID to a wrong class. 
    public TId Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(Entity<TId>? a, Entity<TId>? b)
    {
        return !Equals(a, b);
    }
}