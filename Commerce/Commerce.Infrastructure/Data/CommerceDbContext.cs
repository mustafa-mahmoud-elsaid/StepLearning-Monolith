using Commerce.Domain.Cart;
using Commerce.Domain.Orders;
using Commerce.Domain.Payment;

namespace Commerce.Infrastructure.Data;

public class CommerceDbContext : DbContext
{
    public DbSet<PaymentRecord> PaymentRecords { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

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

            entity.Property(e => e.OrderId)
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

            entity.HasIndex(e => new { e.StudentId, e.OrderId, e.Status });
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StudentId)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            entity.HasIndex(e => e.StudentId)
                .IsUnique();

            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(e => e.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CourseId)
                .IsRequired();

            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.CourseTitle)
                .HasMaxLength(250)
                .IsRequired();

            entity.HasIndex(e => new { e.CartId, e.CourseId })
                .IsUnique();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StudentId)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CourseId)
                .IsRequired();

            entity.Property(e => e.CourseTitle)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });
    }
}
