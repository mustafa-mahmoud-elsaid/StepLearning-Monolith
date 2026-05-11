using Identity.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Application.Infrastructure.Data.Configurations;

internal class StudentConfig : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(s => s.ProfilePictureUrl)
            .HasMaxLength(500);

        builder.Property(s => s.FullName)
            .HasMaxLength(250)
            .IsRequired();
    }
}
