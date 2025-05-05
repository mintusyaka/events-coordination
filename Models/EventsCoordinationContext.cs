using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace events_coordination_frontend.Models;

public partial class EventsCoordinationContext : DbContext
{
    public EventsCoordinationContext()
    {
    }

    public EventsCoordinationContext(DbContextOptions<EventsCoordinationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventOrganizer> EventOrganizers { get; set; }

    public virtual DbSet<EventSponsor> EventSponsors { get; set; }

    public virtual DbSet<EventVolunteer> EventVolunteers { get; set; }

    public virtual DbSet<Organizer> Organizers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PlannedEvent> PlannedEvents { get; set; }

    public virtual DbSet<Sponsor> Sponsors { get; set; }

    public virtual DbSet<SponsorshipType> SponsorshipTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    public virtual DbSet<Volunteer> Volunteers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-Q01DVK6;Database=events_coordination;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__2370F72776EB6B26");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_SetEventStatus");
                    tb.HasTrigger("trg_UpdateEventStatus");
                });

            entity.HasIndex(e => e.StartDate, "idx_start_date");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Description)
                .HasDefaultValue("")
                .HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("planned")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.VenueId).HasColumnName("venue_id");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__organize__3E1D39E1");

            entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__venue_id__3D2915A8");
        });

        modelBuilder.Entity<EventOrganizer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventOrg__3213E83F81BDE1BC");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");

            entity.HasOne(d => d.Event).WithMany(p => p.EventOrganizers)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventOrga__event__44CA3770");

            entity.HasOne(d => d.Organizer).WithMany(p => p.EventOrganizers)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventOrga__organ__45BE5BA9");
        });

        modelBuilder.Entity<EventSponsor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventSpo__3213E83FD1C8C0B5");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.SponsorId).HasColumnName("sponsor_id");
            entity.Property(e => e.SponsorshipId).HasColumnName("sponsorship_id");

            entity.HasOne(d => d.Event).WithMany(p => p.EventSponsors)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventSpon__event__4C6B5938");

            entity.HasOne(d => d.Sponsor).WithMany(p => p.EventSponsors)
                .HasForeignKey(d => d.SponsorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventSpon__spons__4D5F7D71");

            entity.HasOne(d => d.Sponsorship).WithMany(p => p.EventSponsors)
                .HasForeignKey(d => d.SponsorshipId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventSpon__spons__4E53A1AA");
        });

        modelBuilder.Entity<EventVolunteer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventVol__3213E83F8DA361BB");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.VolunteerId).HasColumnName("volunteer_id");

            entity.HasOne(d => d.Event).WithMany(p => p.EventVolunteers)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventVolu__event__489AC854");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.EventVolunteers)
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventVolu__volun__498EEC8D");
        });

        modelBuilder.Entity<Organizer>(entity =>
        {
            entity.HasKey(e => e.OrganizerId).HasName("PK__Organize__06347014A5602ADB");

            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.Organization)
                .HasMaxLength(100)
                .HasColumnName("organization");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Organizers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Organizer__user___787EE5A0");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EAB2282DC2");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.PayerId).HasColumnName("payer_id");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("payment_time");
            entity.Property(e => e.RecipientId).HasColumnName("recipient_id");

            entity.HasOne(d => d.Payer).WithMany(p => p.PaymentPayers)
                .HasForeignKey(d => d.PayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__payer___1F98B2C1");

            entity.HasOne(d => d.Recipient).WithMany(p => p.PaymentRecipients)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__recipi__208CD6FA");
        });

        modelBuilder.Entity<PlannedEvent>(entity =>
        {
            entity.HasKey(e => e.PlannedEventId).HasName("PK__PlannedE__C04E7FB03DCC1349");

            entity.HasIndex(e => e.EventId, "idx_event_id");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.Property(e => e.PlannedEventId).HasColumnName("planned_event_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.PlannedEvents)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlannedEv__event__40F9A68C");

            entity.HasOne(d => d.User).WithMany(p => p.PlannedEvents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlannedEv__user___41EDCAC5");
        });

        modelBuilder.Entity<Sponsor>(entity =>
        {
            entity.HasKey(e => e.SponsorId).HasName("PK__Sponsors__BE37D4543705DC05");

            entity.Property(e => e.SponsorId).HasColumnName("sponsor_id");
            entity.Property(e => e.Organization)
                .HasMaxLength(100)
                .HasColumnName("organization");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Sponsors)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Sponsors__user_i__7E37BEF6");

            entity.HasMany(d => d.Sponsorships).WithMany(p => p.Sponsors)
                .UsingEntity<Dictionary<string, object>>(
                    "SponsorsSponsorshipType",
                    r => r.HasOne<SponsorshipType>().WithMany()
                        .HasForeignKey("SponsorshipId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SponsorsS__spons__04E4BC85"),
                    l => l.HasOne<Sponsor>().WithMany()
                        .HasForeignKey("SponsorId")
                        .HasConstraintName("FK__SponsorsS__spons__03F0984C"),
                    j =>
                    {
                        j.HasKey("SponsorId", "SponsorshipId").HasName("PK__Sponsors__D1DF8D577541EDF0");
                        j.ToTable("SponsorsSponsorshipTypes");
                        j.HasIndex(new[] { "SponsorId", "SponsorshipId" }, "UQ_SponsorsSponsorshipTypes").IsUnique();
                        j.IndexerProperty<int>("SponsorId").HasColumnName("sponsor_id");
                        j.IndexerProperty<int>("SponsorshipId").HasColumnName("sponsorship_id");
                    });
        });

        modelBuilder.Entity<SponsorshipType>(entity =>
        {
            entity.HasKey(e => e.SponsorshipId).HasName("PK__Sponsors__FE8590378BDACE23");

            entity.Property(e => e.SponsorshipId).HasColumnName("sponsorship_id");
            entity.Property(e => e.SponsorshipType1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sponsorship_type");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FC80A03D9");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Balance)
                .HasDefaultValueSql("((0.0))")
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venues__82A8BE8D971C906B");

            entity.Property(e => e.VenueId).HasColumnName("venue_id");
            entity.Property(e => e.Building)
                .HasMaxLength(4)
                .HasColumnName("building");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.City)
                .HasMaxLength(20)
                .HasColumnName("city");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Street)
                .HasMaxLength(20)
                .HasColumnName("street");
            entity.Property(e => e.VenueType)
                .HasMaxLength(50)
                .HasColumnName("venue_type");
        });

        modelBuilder.Entity<Volunteer>(entity =>
        {
            entity.HasKey(e => e.VolunteerId).HasName("PK__Voluntee__0FE766B19A2CBD8C");

            entity.Property(e => e.VolunteerId).HasColumnName("volunteer_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Volunteers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Volunteer__user___7B5B524B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
