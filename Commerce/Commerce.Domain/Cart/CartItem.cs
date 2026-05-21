namespace Commerce.Domain.Cart;

public class CartItem
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid CourseId { get; private set; }
    public decimal Price { get; private set; }
    public string CourseTitle { get; private set; } = string.Empty;

    public static CartItem Create(Guid cartId, Guid courseId, decimal price, string courseTitle)
    {
        if (cartId == Guid.Empty)
            throw new InvalidOperationException("Cart id must not be empty");

        if (courseId == Guid.Empty)
            throw new InvalidOperationException("Course id must not be empty");

        if (price < 0)
            throw new InvalidOperationException("Price must not be negative");

        if (string.IsNullOrWhiteSpace(courseTitle))
            throw new InvalidOperationException("Course title is required");

        return new()
        {
            Id = Guid.NewGuid(),
            CartId = cartId,
            CourseId = courseId,
            Price = price,
            CourseTitle = courseTitle
        };
    }
}
