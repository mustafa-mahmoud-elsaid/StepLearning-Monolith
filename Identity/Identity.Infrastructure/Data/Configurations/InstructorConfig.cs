using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data.Configurations;

internal class InstructorConfig : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.Property(s => s.ProfilePictureUrl)
            .HasMaxLength(500);

        builder.Property(s => s.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.LastName)
            .HasMaxLength(50)
            .IsRequired();
    }
}
