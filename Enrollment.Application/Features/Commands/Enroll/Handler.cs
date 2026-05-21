using Enrollment.Application.Repositories;
using MediatR;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Events;
using StepLearning.Shared.Result;

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
        var dto = request.dto;

        var validStudent = await studentService.Exists(dto.StudentId); 

        if(!validStudent)
            return Result.Failure("Failed to enroll, student not exists");

        var enrollmentsToCreate = new List<Domain.Entities.Enrollment>();

        foreach (var courseId in dto.CourseIds)
        {
            var validCourse = await courseService.Exists(courseId);
            if (!validCourse)
                return Result.Failure($"Failed to enroll, course {courseId} not exists");

            var isEnrolled = await enrollmentRepository.IsEnrolled(dto.StudentId, courseId, cancellationToken);
            if (isEnrolled)
                continue;

            try
            {
                var enrollment = Domain.Entities.Enrollment.Create(dto.StudentId, courseId, dto.PaymentId, dto.Status);
                enrollmentsToCreate.Add(enrollment);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        foreach (var enrollment in enrollmentsToCreate)
        {
            await enrollmentRepository.AddEnrollment(enrollment, cancellationToken);

            await integrationEventPublisher.PublishAsync(
                new EnrollmentCompletedEvent(
                    enrollment.Id,
                    enrollment.StudentId,
                    enrollment.CourseId,
                    enrollment.PaymentId,
                    DateTime.UtcNow),
                cancellationToken);
        }

        return Result.Success();
    }
}
