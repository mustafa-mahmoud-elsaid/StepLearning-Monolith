namespace Courses.Domain.Entities;

public class VideoItem : SectionItem
{
    public string VideoUrl { get; private set; } = string.Empty;
    public TimeSpan Duration { get; private set; }

    private VideoItem() { }

    public static VideoItem Create(string title, string videoUrl, TimeSpan duration, Guid sectionId, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(videoUrl))
            throw new ArgumentException("VideoUrl cannot be empty.", nameof(videoUrl));
        if (sectionId == Guid.Empty)
            throw new ArgumentException("SectionId cannot be empty.", nameof(sectionId));

        return new VideoItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            VideoUrl = videoUrl,
            Duration = duration,
            SectionId = sectionId,
            DisplayOrder = displayOrder
        };
    }
}
