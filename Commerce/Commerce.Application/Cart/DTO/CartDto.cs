namespace Commerce.Application.Cart.DTO;

public sealed record CartDto(
    Guid StudentId,
    IReadOnlyCollection<CartItemDto> Items,
    decimal TotalAmount);

public sealed record CartItemDto(
    Guid CourseId,
    string CourseTitle,
    decimal Price);
