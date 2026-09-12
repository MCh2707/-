using Microsoft.EntityFrameworkCore;

namespace BAGEBI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Kindergarten> Kindergartens => Set<Kindergarten>();
    public DbSet<User> Users => Set<User>();
    public DbSet<DailyAttendance> DailyAttendances => Set<DailyAttendance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Kindergarten>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasOne(e => e.Kindergarten)
                  .WithMany(k => k.Users)
                  .HasForeignKey(e => e.KindergartenId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<DailyAttendance>(entity =>
        {
            entity.HasKey(e => e.Id);
            // One record per kindergarten per calendar day
            entity.HasIndex(e => new { e.KindergartenId, e.Date }).IsUnique();
            entity.HasOne(e => e.Kindergarten)
                  .WithMany(k => k.DailyAttendances)
                  .HasForeignKey(e => e.KindergartenId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
