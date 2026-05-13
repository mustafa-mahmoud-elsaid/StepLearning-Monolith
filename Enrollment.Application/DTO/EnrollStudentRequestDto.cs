using Enrollment.Application.Domain.Enums;

namespace Enrollment.Application.DTO;

public record EnrollStudentRequestDto(Guid StudentId, Guid CourseId, EnrollmentStatus Status);