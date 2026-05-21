namespace Courses.Application.DTO;

public record ReorderDto(Guid ItemId, Guid? PreviousItemId, Guid? NextItemId);
