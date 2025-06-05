namespace ViaEventAssociation.Core.Domain.Common.Bases;

// The main reason for this class to exist is to:
// standardize equality comparison by value, not by reference
public abstract class ValueObject
{
    // Could be just an object, but a Value Object may have multiple properties we might want to compare in Equals(). 
    // Specifically f.x. EventTimeRange has: startTime: DateTime & endTime: DateTime
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        // This compares all the equality components of the current object and the other object in order, one by one.
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    // If your object does not override it correctly, you can get inconsistent behavior,
    // e.g., Equal(EmailAddress1, EmailAddress2) can return false, despite their Value is identical.
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
    }

    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(ValueObject? a, ValueObject? b)
    {
        return !Equals(a, b);
    }

    public new abstract string ToString();
}