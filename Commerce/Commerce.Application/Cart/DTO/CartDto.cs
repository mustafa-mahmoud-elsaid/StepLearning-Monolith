namespace Commerce.Application.Cart.DTO;

public sealed record CartDto(
    IReadOnlyCollection<CartItemDto> Items,
    decimal TotalAmount);

public sealed record CartItemDto(
    Guid CourseId,
    string CourseTitle,
    decimal Price);
