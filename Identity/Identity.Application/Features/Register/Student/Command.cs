using Identity.Application.DTO;

namespace Identity.Application.Features.Register.Student;

public record StudentRegisterCommand(StudentRegisterDto Credentials) : IRequest<Result<LoginResponse>>;
