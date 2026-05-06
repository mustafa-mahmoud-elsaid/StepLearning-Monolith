using System.Runtime.InteropServices;

namespace Courses.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public bool IsPublished { get; private set; }
    public Guid InstructorId { get; private set; }
    private readonly List<Section> _sections = new();
    public IReadOnlyCollection<Section> Sections => _sections.AsReadOnly();
    public static Course Create(string title, string? description, Guid instructorId)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Title = title,
            InstructorId = instructorId,
            Description = description,
            IsPublished = false,
            Price = 0m,
        };
    }
}
