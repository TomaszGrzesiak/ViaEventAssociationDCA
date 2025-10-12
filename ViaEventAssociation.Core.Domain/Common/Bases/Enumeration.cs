namespace ViaEventAssociation.Core.Domain.Common.Bases
{
    public abstract class Enumeration
    {
        public int Id { get; }
        public string Name { get; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

        public override bool Equals(object? obj)
        {
            if (obj is not Enumeration otherValue)
                return false; // If obj js Enumeration, then it gets assigned to a new variable: otherValue of type Enumeration
            return GetType().Equals(obj.GetType()) && Id.Equals(otherValue.Id); // GetType() is the same as this.GetType()
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            // This allows the method to find all public static fields of that type (like in status: Pending, Accepted, Declined)
            // and Return them as a collection of f.x. InvitationStatus
            return typeof(T)
                .GetFields(System.Reflection.BindingFlags.Public |
                           System.Reflection.BindingFlags.Static |
                           System.Reflection.BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(T))
                .Select(f => f.GetValue(null))
                .Cast<T>();
        }
    }
}