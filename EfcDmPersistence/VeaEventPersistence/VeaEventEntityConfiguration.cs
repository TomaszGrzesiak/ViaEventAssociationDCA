using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;

namespace EfcDmPersistence.VeaEventPersistence;

public class VeaEventEntityConfiguration: IEntityTypeConfiguration<VeaEvent>
{
    public void Configure(EntityTypeBuilder<VeaEvent> b)
    {
        
        b.HasKey(e => e.Id); // Configuring EventId as a PK of VeaEvent class
        
        b.Property(e => e.Id)
            .HasConversion(
                eventId => eventId!.Value,
                dbValue => EventId.FromGuid(dbValue)
                );
        // You can ignore complex stuff for the FIRST script if needed:
        b.Ignore(e => e.Invitations);
        b.Ignore(e => e.Description);
        b.Ignore(e => e.Status);
        b.Ignore(e => e.TimeRange);
        b.Ignore(e => e.Title);
        b.Ignore(e => e.Visibility);
        b.Ignore(e => e.MaxGuestsNo);
        b.Ignore(e => e.GuestList);
        
        // // Below ChatGPT generated proposition to fill the method:
        //
        // // PK on strong ID with a converter (EventId <-> Guid)
        // b.HasKey(e => e.Id);
        // b.Property(e => e.Id)
        //     .ValueGeneratedNever()
        //     .HasConversion(
        //         id => id.Value,
        //         g  => EventId.FromGuid(g));
        //
        // // Map a couple of scalars so EF is satisfied
        // b.Property(e => e.Title)
        //     .HasConversion(
        //         t => t.Value,
        //         s => EventTitle.Create(s).Payload!)
        //     .HasMaxLength(75)
        //     .IsRequired();
        //
        // b.Property(e => e.Description)
        //     .HasConversion(
        //         d => d.Value ?? string.Empty,
        //         s => EventDescription.Create(s).Payload!)
        //     .HasMaxLength(250)
        //     .IsRequired();
        //
        // // Use FIELD ACCESS for the backing field "_invitations"
        // var invNav = b.Metadata.FindNavigation(nameof(VeaEvent.Invitations));
        // invNav!.SetPropertyAccessMode(PropertyAccessMode.Field);
        //
        // b.OwnsMany<Invitation>("_invitations", inv =>
        // {
        //     inv.ToTable("Invitations");
        //     inv.WithOwner().HasForeignKey("EventId");
        //
        //     // PRIMARY KEY for the owned entity = Invitation.Id (your value object)
        //     inv.HasKey(i => i.Id);
        //
        //     // Map Invitation.Id (value object) <-> Guid
        //     inv.Property(i => i.Id)
        //         .HasConversion(id => id.Value, g => InvitationId.FromGuid(g))
        //         .HasColumnName("InvitationId")
        //         .ValueGeneratedNever()      // important: you create it in domain
        //         .IsRequired();
        //
        //     // Map GuestId (value object) <-> Guid
        //     inv.Property(i => i.GuestId)
        //         .HasConversion(id => id.Value, g => GuestId.FromGuid(g))
        //         .IsRequired();
        //
        //     // Map smart-enum status <-> int
        //     inv.Property(i => i.Status)
        //         .HasConversion(s => s.Id, id => InvitationStatus.FromId(id).Payload!)
        //         .IsRequired();
        // });
    }
}