using Identity.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Register.Instructor;

public record InstructorRegisterCommand(InstructorRegisterDto Credentials):IRequest<Result<LoginResponse>>;
