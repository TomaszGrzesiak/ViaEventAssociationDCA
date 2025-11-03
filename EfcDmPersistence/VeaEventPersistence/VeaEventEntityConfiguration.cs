using System.Runtime.CompilerServices;
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
        
        b.ToTable("VeaEvents"); // just to show off that I can name the table whatever I want 
        b.HasKey(e => e.Id); // Configuring EventId as a PK of VeaEvent class
        b.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(
                eventId => eventId!.Value,
                dbValue => EventId.FromGuid(dbValue)
                );
        
        //b.Ignore(e => e.Title);
        b.OwnsOne<EventTitle>(e => e.Title)
            .Property(vo => vo.Value)
            .HasMaxLength(75)
            .IsRequired();
        b.Navigation(e => e.Title).IsRequired();
        
        //b.Ignore(e => e.Description);
        b.OwnsOne(e => e.Description, ownedNavigationBuilder =>
        {
            ownedNavigationBuilder.Property(vo => vo.Value)
                .HasMaxLength(250)
                .IsRequired();
        });
        b.Navigation(e => e.Description).IsRequired();
        
        //b.Ignore(e => e.MaxGuestsNo);
        // example of a ComplexProperty (better use HasConversion... but just as an exercise...)
        b.ComplexProperty(e => e.MaxGuestsNo,
            propBuilder => propBuilder.Property(vo => vo.Value).IsRequired()
        );
        
        //b.Ignore(e => e.TimeRange);
        b.OwnsOne(e => e.TimeRange, propBuilder =>
        {
            propBuilder.Property(vo => vo.StartTime);
            propBuilder.Property(vo => vo.EndTime);
        });
        b.Navigation(e => e.TimeRange).IsRequired(false);
        
        // b.Ignore(e => e.Status);
        // --- Smart enums (int-backed) ---
        b.Property(e => e.Status)
            .HasConversion(s => s.Id, id => EventStatus.FromId(id).Payload!)
            .IsRequired();

        // b.Ignore(e => e.Visibility);
        b.Property(e => e.Visibility)
            .HasConversion(v => v.Id, id => EventVisibility.FromId(id).Payload!)
            .IsRequired();

        
        // b.Ignore(e => e.GuestList);
        // FIELD ACCESS so EF populates your backing list. It's because EF can't set a read-only property when shaping a class while reading from DB.
        // these instructions tell EF that it should populate the private field _guestList instead of trying to populate GuestList property.
        var guestNav = b.Metadata.FindNavigation(nameof(VeaEvent.GuestList));
        guestNav!.SetField("_guestList");                                 // tie to backing field
        guestNav.SetPropertyAccessMode(PropertyAccessMode.Field);         // use field access

        b.OwnsMany(e => e.GuestList, part =>
        {
            part.ToTable("EventParticipants");
            part.WithOwner().HasForeignKey("EventId");

            // persist the scalar Guid inside the VO; column name is just SQL, EF property name remains "Value"
            part.Property(p => p.Value)
                .HasColumnName("GuestId")
                .IsRequired();

            // composite PK must use EF property names, not column names
            part.HasKey("EventId", "Value");

            // optional (redundant with composite PK):
            // part.HasIndex("EventId", "Value").IsUnique();
        });
        
        // b.Ignore(e => e.Invitations);
        var invNav = b.Metadata.FindNavigation(nameof(VeaEvent.Invitations));
        invNav!.SetField("_invitations");
        invNav.SetPropertyAccessMode(PropertyAccessMode.Field);

        b.OwnsMany(e => e.Invitations, inv =>
        {
            inv.ToTable("Invitations");
            inv.WithOwner().HasForeignKey("EventId");

            inv.HasKey(i => i.Id);
            inv.Property(i => i.Id)
                .HasConversion(id => id.Value, g => InvitationId.FromGuid(g))
                .HasColumnName("InvitationId")
                .ValueGeneratedNever()
                .IsRequired();

            inv.Property(i => i.GuestId)
                .HasConversion(id => id.Value, g => GuestId.FromGuid(g))
                .IsRequired();

            inv.Property(i => i.Status)
                .HasConversion(s => s.Id, id => InvitationStatus.FromId(id).Payload!)
                .IsRequired();

            // 🔒 Enforce the invariant at the database boundary
            inv.HasIndex("EventId", nameof(Invitation.GuestId)).IsUnique();
        });
    }
}