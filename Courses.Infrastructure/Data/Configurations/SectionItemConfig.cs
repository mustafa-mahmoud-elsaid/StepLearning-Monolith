using Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Data.Configurations;

internal class SectionItemConfig : IEntityTypeConfiguration<SectionItem>
{
    public void Configure(EntityTypeBuilder<SectionItem> builder)
    {
        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.Property(c => c.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.DisplayOrder)
            .IsRequired();

        builder.HasIndex(s => new { s.SectionId, s.DisplayOrder })
            .IsUnique()
            .HasFilter("IsDeleted = 0");

        builder.HasDiscriminator<string>("ItemType")
            .HasValue<VideoItem>("Video")
            .HasValue<PdfItem>("Pdf");

    }
}
internal class VideoItemConfig : IEntityTypeConfiguration<VideoItem>
{
    public void Configure(EntityTypeBuilder<VideoItem> builder)
    {
        builder.Property(v => v.VideoUrl)
            .HasMaxLength(1000)
            .IsRequired();
    }
}
internal class PdfItemConfig : IEntityTypeConfiguration<PdfItem>
{
    public void Configure(EntityTypeBuilder<PdfItem> builder)
    {
        builder.Property(p => p.FileUrl)
            .HasMaxLength(150)
            .IsRequired();
    }
}