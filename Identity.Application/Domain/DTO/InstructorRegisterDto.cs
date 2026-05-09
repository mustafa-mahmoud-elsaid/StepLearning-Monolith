namespace Identity.Application.Domain.DTO;

public record InstructorRegisterDto(string FirstName, string LastName, string? Bio, string? ProfilePictureUrl, string Email, string Password);