namespace ViaEventAssociation.Core.Domain.Common.Bases
{
    public abstract class Enumeration: IEquatable<Enumeration>
    {
        public int Id { get; }
        public string? Name { get; }

        protected Enumeration() { } // for EF only

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString() => Name?? "";

        public override bool Equals(object? obj)  => Equals(obj as Enumeration);
        
        public bool Equals(Enumeration? other) => other is not null && other.GetType() == GetType() && other.Id == Id;
        
        public override int GetHashCode() => HashCode.Combine(GetType(), Id);
        
        public static bool operator ==(Enumeration? left, Enumeration? right) => Equals(left, right);
        public static bool operator !=(Enumeration? left, Enumeration? right) => !Equals(left, right);

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