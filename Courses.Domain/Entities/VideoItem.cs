namespace Courses.Domain.Entities;

public class VideoItem : SectionItem
{
    public string VideoUrl { get; set; } = string.Empty;
    public TimeSpan DurationInMinutes { get; set; }
}
