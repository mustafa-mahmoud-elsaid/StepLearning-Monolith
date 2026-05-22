using Enrollment.Domain.Enums;

namespace Enrollment.Application.DTO;

public record EnrollStudentRequestDto(Guid StudentId, List<Guid> CourseIds, Guid PaymentId, EnrollmentStatus Status);
