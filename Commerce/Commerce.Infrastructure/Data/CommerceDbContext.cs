using Commerce.Application.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Infrastructure.Data;

public class CommerceDbContext : DbContext
{
    public DbSet<PaymentRecord> PaymentRecords { get; set; }

    public CommerceDbContext(DbContextOptions<CommerceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentRecord>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StudentId)
                .IsRequired();

            entity.Property(e => e.CourseId)
                .IsRequired();

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(e => e.ProviderReference)
                .HasMaxLength(500);

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(e => new { e.StudentId, e.CourseId, e.Status });
        });
    }
}
