namespace Identity.Application.Domain.Entities;

public class Student
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? ProfilePictureUrl { get; private set; }

    public static Student Create(string fullName,  Guid userId, DateOnly? dateOfBirth, string? profilePictureUrl)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidOperationException("fullName must have actual value");
        }
        if (userId == Guid.Empty)
        {
            throw new InvalidOperationException("User id must not be empty");
        }
        return new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DateOfBirth = dateOfBirth,
            ProfilePictureUrl = profilePictureUrl
        };
    }
}
