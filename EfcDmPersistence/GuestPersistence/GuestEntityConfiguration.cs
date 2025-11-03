using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;

namespace EfcDmPersistence.GuestPersistence;

public class GuestEntityConfiguration: IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> b)
    {
        b.ToTable("Guests");

        // Strong ID (you create it, DB must not)
        b.HasKey(g => g.Id);
        b.Property(g => g.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, g => GuestId.FromGuid(g));

        // Value objects (all required in your domain)
        b.Property(g => g.Email)
            .HasConversion(v => v.Value, s => EmailAddress.Create(s).Payload!)
            .HasMaxLength(80)
            .IsRequired();

        b.Property(g => g.FirstName)
            .HasConversion(v => v.Value, s => GuestName.Create(s).Payload!)
            .HasMaxLength(25)
            .IsRequired();

        b.Property(g => g.LastName)
            .HasConversion(v => v.Value, s => GuestName.Create(s).Payload!)
            .HasMaxLength(25)
            .IsRequired();

        b.Property(g => g.ProfilePictureUrlAddress)
            .HasConversion(v => v.Value, s => ProfilePictureUrl.Create(s).Payload!)
            .HasMaxLength(500)
            .IsRequired();

        // Optional but sensible:
        b.HasIndex(g => g.Email).IsUnique();
    }
}