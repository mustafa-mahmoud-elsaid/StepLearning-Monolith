using Enrollment.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Enrollment.Application.Features.Commands.Enroll;

public record EnrollStudentCommand(EnrollStudentRequestDto dto) : IRequest<Result>;
