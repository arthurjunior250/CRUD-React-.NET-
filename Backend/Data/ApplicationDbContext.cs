using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database creation error: {ex.Message}");
            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Item>(entity =>
        {
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(true);
            
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("UTC_TIMESTAMP()")
                .ValueGeneratedOnAdd();

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasIndex(e => e.CreatedAt);
        });
    }

    public DbSet<Item> Items { get; set; }
} 