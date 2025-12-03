using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EfcQueries.Models;

public partial class VeaReadModelContext : DbContext
{
    public VeaReadModelContext()
    {
    }

    public VeaReadModelContext(DbContextOptions<VeaReadModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<EventParticipant> EventParticipants { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<VeaEvent> VeaEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=..\\EfcDmPersistence\\vea_write.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.GuestId });

            entity.HasOne(d => d.Event).WithMany(p => p.EventParticipants).HasForeignKey(d => d.EventId);
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Guests_Email").IsUnique();
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasIndex(e => new { e.EventId, e.GuestId }, "IX_Invitations_EventId_GuestId").IsUnique();

            entity.HasOne(d => d.Event).WithMany(p => p.Invitations).HasForeignKey(d => d.EventId);
        });

        modelBuilder.Entity<VeaEvent>(entity =>
        {
            entity.Property(e => e.DescriptionValue).HasColumnName("Description_Value");
            entity.Property(e => e.MaxGuestsNoValue).HasColumnName("MaxGuestsNo_Value");
            entity.Property(e => e.TimeRangeEndTime).HasColumnName("TimeRange_EndTime");
            entity.Property(e => e.TimeRangeStartTime).HasColumnName("TimeRange_StartTime");
            entity.Property(e => e.TitleValue).HasColumnName("Title_Value");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
