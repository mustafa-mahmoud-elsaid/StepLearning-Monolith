namespace Courses.Domain.Entities;

public abstract class SectionItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public Guid SectionId { get; set; }
    public Section Section { get; set; } = null!;
    public int DisplayOrder { get; set; }
}
