using EventEase.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One Event -> many Registrations. Deleting an event deletes its registrations.
        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Event)
            .WithMany(e => e.Registrations)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Duplicate prevention at DATABASE level: same email cannot join the same event twice.
        modelBuilder.Entity<Registration>()
            .HasIndex(r => new { r.EventId, r.Email })
            .IsUnique();

        // Store attendance as readable text ("Present") instead of a number.
        modelBuilder.Entity<Registration>()
            .Property(r => r.Attendance)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Helpful indexes for searching / sorting.
        modelBuilder.Entity<Event>().HasIndex(e => e.EventDate);
        modelBuilder.Entity<Event>().HasIndex(e => e.Category);
    }
}
