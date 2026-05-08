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
    // TODO: Add Slug (string) property — auto-generated from Title, unique, used for SEO-friendly URLs.
    // TODO: Add Ratings navigation property and AverageRating computed/denormalized field.
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

    public void Publish()
    {
        if (IsPublished)
            throw new InvalidOperationException("Course is already published.");

        var hasPublishedContent = _sections.Any(s => s.IsPublished && s.SectionItems.Any());

        if (!hasPublishedContent)
            throw new InvalidOperationException("Course must have at least one published section with items before publishing.");

        IsPublished = true;
    }

    public void Unpublish()
    {
        if (!IsPublished)
            throw new InvalidOperationException("Course is not published.");

        IsPublished = false;
    }

    public void Update(string? title, string? description, decimal? price)
    {
        if (title is not null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            Title = title;
        }

        if (price.HasValue)
        {
            if (price.Value < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(price));

            Price = price.Value;
        }

        Description = description ?? Description;
    }
}
