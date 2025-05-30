namespace ViaEventAssociation.Core.Domain.Common.Bases;

// "Session 2 - DDD.pptx" slide 78. Copy - pasted from the slide.
public abstract class AggregateRoot<TId> : Entity<TId> where TId : ValueObject
{
    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}