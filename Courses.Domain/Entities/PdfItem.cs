namespace Courses.Domain.Entities;

public class PdfItem : SectionItem
{
    public string FileUrl { get; private set; } = string.Empty;

    private PdfItem() { }

    public static PdfItem Create(string title, string fileUrl, Guid sectionId, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("FileUrl cannot be empty.", nameof(fileUrl));
        if (sectionId == Guid.Empty)
            throw new ArgumentException("SectionId cannot be empty.", nameof(sectionId));

        return new PdfItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            FileUrl = fileUrl,
            SectionId = sectionId,
            DisplayOrder = displayOrder
        };
    }
}