namespace Identity.Application.Domain.Entities;

public class Instructor
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string FullName => string.Concat(FirstName, " ", LastName);
    public string? Bio { get; set; }

}
