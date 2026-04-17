using Microsoft.EntityFrameworkCore;
using PplTracker.Core.Models;

namespace PplTracker.Data.Context;

public class PplTrackerDbContext : DbContext
{
    public PplTrackerDbContext(DbContextOptions<PplTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<Person> People { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<PersonLocation> PersonLocations { get; set; }
    public DbSet<Schedule> Schedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Ignore(e => e.FullName);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<PersonLocation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Person)
                  .WithMany(p => p.PersonLocations)
                  .HasForeignKey(e => e.PersonId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Location)
                  .WithMany(l => l.PersonLocations)
                  .HasForeignKey(e => e.LocationId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasOne(e => e.Person)
                  .WithMany(p => p.Schedules)
                  .HasForeignKey(e => e.PersonId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Location)
                  .WithMany(l => l.Schedules)
                  .HasForeignKey(e => e.LocationId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
