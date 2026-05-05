using Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Data.Configurations;

internal class SectionConfig : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.Property(c => c.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(c => c.DisplayOrder)
            .IsRequired();

        builder.HasIndex(s => new { s.CourseId, s.DisplayOrder })
            .IsUnique()
            .HasFilter("IsDeleted = 0");

        builder.HasMany(c => c.SectionItems)
            .WithOne(s => s.Section)
            .HasForeignKey(s => s.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
