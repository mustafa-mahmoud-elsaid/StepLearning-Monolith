namespace Courses.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public Guid InstructorId { get; set; }
    public ICollection<Section> Sections { get; set; } = new List<Section>();
}
