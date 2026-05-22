using Microsoft.EntityFrameworkCore;
namespace Enrollment.Infrastructure.Data;

public sealed class EnrollmentDbContext : DbContext
{

    public DbSet<Domain.Entities.Enrollment> Enrollments { get; set; }
    public EnrollmentDbContext(DbContextOptions<EnrollmentDbContext> options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Domain.Entities.Enrollment>(optins =>
        {
            optins.Property(e => e.Status).HasConversion<string>();

            optins.Property(e => e.PaymentId)
                .IsRequired();

            optins.HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique();
        });
    }
}
