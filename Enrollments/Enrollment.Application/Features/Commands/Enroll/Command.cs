using Enrollment.Application.DTO;

namespace Enrollment.Application.Features.Commands.Enroll;

public record EnrollStudentCommand(EnrollStudentRequestDto Details) : IRequest<Result>;
