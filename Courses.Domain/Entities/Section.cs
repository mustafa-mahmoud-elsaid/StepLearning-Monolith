namespace Courses.Domain.Entities;

public class Section : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public ICollection<SectionItem> SectionItems { get; set; } = new List<SectionItem>();
}
