using Identity.Application.DTO;

namespace Identity.Application.Features.Register.Instructor;

public record InstructorRegisterCommand(InstructorRegisterDto Credentials):IRequest<Result<LoginResponse>>;
