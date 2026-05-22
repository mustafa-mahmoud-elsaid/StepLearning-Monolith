namespace Identity.Application.DTO;

public record StudentRegisterDto(string FullName, DateOnly? DateOfBirth, string? ProfilePictureUrl, string Email, string Password);
