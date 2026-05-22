using Courses.Domain.Entities;

namespace Courses.Infrastructure.Data;

public class CoursesDbContext : DbContext
{
    public CoursesDbContext(DbContextOptions<CoursesDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<SectionItem> SectionItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoursesDbContext).Assembly);
    }
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
               entry.Property(e => e.CreatedAt).CurrentValue = DateTimeOffset.UtcNow;
               entry.Property(e => e.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
               entry.Property(e => e.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
            }
        }

        return base.SaveChangesAsync(ct);
    }
}
