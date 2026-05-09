using Identity.Application.Domain.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Register.Instructor;

public record InstructorRegisterCommand(InstructorRegisterDto dto):IRequest<Result<LoginResponse>>;
