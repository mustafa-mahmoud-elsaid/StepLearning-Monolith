namespace Identity.Application.Domain.DTO;

public record StudentRegisterDto(string FullName, DateOnly? DateOfBirth, string? ProfilePictureUrl, string Email, string Password);