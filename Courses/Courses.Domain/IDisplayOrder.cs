namespace Courses.Domain;

public interface IDisplayOrder
{
    int DisplayOrder { get; }
    void UpdateDisplayOrder(int newOrder);
}
