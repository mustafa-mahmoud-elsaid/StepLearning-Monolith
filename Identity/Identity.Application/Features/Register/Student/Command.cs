using Identity.Application.Domain.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Register.Student;

public record StudentRegisterCommand(StudentRegisterDto Credentials) : IRequest<Result<LoginResponse>>;
