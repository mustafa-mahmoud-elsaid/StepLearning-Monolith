namespace Courses.Domain.Entities;

public class Section : BaseEntity, IDisplayOrder
{
    public string Title { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public bool IsPublished { get; private set; }
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public ICollection<SectionItem> SectionItems { get; private set; } = new List<SectionItem>();

    private Section() { }

    public static Section Create(string title, Guid courseId, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (courseId == Guid.Empty)
            throw new ArgumentException("CourseId cannot be empty.", nameof(courseId));

        return new Section
        {
            Id = Guid.NewGuid(),
            Title = title,
            CourseId = courseId,
            DisplayOrder = displayOrder,
            IsPublished = false
        };
    }

    public void UpdateDisplayOrder(int newOrder)
    {
        DisplayOrder = newOrder;
    }

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Title cannot be empty.", nameof(newTitle));
        Title = newTitle;
    }
}
