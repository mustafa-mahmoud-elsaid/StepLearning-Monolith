using Enrollment.Application.Repositories;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Events;

namespace Enrollment.Application.Features.Commands.Enroll;

internal sealed class Handler(
    ICourseService courseService,
    IStudentService studentService,
    IEnrollmentRepository enrollmentRepository,
    IIntegrationEventPublisher integrationEventPublisher
    ) : IRequestHandler<EnrollStudentCommand, Result>
{
    public async Task<Result> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Details;

        var studentEmail = await studentService.GetEmail(dto.StudentId, cancellationToken); 

        if (string.IsNullOrWhiteSpace(studentEmail))
            return Result.Failure("Failed to enroll. Student not found.");

        var enrollmentsToCreate = new List<(Domain.Entities.Enrollment Enrollment, string CourseName, string? ThumbnailUrl)>();

        foreach (var courseId in dto.CourseIds)
        {
            var courseSnapshot = await courseService.GetSnapshot(courseId, cancellationToken);
            if (courseSnapshot is null)
                return Result.Failure($"Failed to enroll, Course {courseId} not found or is not available for purchase.");

            var isEnrolled = await enrollmentRepository.IsEnrolled(dto.StudentId, courseId, cancellationToken);
            if (isEnrolled)
                continue;

            try
            {
                var enrollment = Domain.Entities.Enrollment.Create(dto.StudentId, courseId, dto.PaymentId, dto.Status);
                enrollmentsToCreate.Add((enrollment, courseSnapshot.Title, courseSnapshot.ThumbnailUrl));
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        if (enrollmentsToCreate.Any())
        {
            await enrollmentRepository.AddEnrollments(enrollmentsToCreate.Select(x => x.Enrollment), cancellationToken);

            var coursesDetails = enrollmentsToCreate
                .Select(x => new EnrolledCourseDetails(x.CourseName, x.ThumbnailUrl))
                .ToList();

            await integrationEventPublisher.PublishAsync(
                new EnrollmentCompletedEvent(
                    studentEmail,
                    coursesDetails,
                    DateTime.UtcNow),
                cancellationToken);
        }

        return Result.Success();
    }
}
