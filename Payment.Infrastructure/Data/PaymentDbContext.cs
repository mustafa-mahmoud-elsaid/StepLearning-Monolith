using Microsoft.EntityFrameworkCore;
using Payment.Application.Domain.Entities;

namespace Payment.Infrastructure.Data;

public class PaymentDbContext : DbContext
{
    public DbSet<PaymentRecord> PaymentRecords { get; set; }

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Basic configuration
        modelBuilder.Entity<PaymentRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        });
    }
}
