namespace Courses.Domain.Entities;

public abstract class SectionItem : BaseEntity, IDisplayOrder
{
    public string Title { get; protected set; } = string.Empty;
    public Guid SectionId { get; protected set; }
    public Section Section { get; protected set; } = null!;
    public int DisplayOrder { get; protected set; }

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
