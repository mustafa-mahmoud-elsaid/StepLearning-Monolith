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

        // validate on user and course id
        var validCourse = await courseService.Exists(dto.CourseId);

        if (!validCourse)
            return Result.Failure("Failed to enroll, courses not exists");

        var validStudent = await studentService.Exists(dto.StudentId); 

        if(!validStudent)
            return Result.Failure("Failed to enroll, student not exists");

        var isEnrolled =  await enrollmentRepository.IsEnrolled(dto.StudentId, dto.CourseId, cancellationToken);

        if (isEnrolled)
            return Result.Failure("Student is already enrolled");

        // TODO:check if the user payment succeeded


        Domain.Entities.Enrollment enrollment;
        try
        {
            enrollment = Domain.Entities.Enrollment.Create(dto.StudentId, dto.CourseId, dto.PaymentId, dto.Status);
        }
        catch (InvalidOperationException ex)
        {

            return Result.Failure(ex.Message);
        }


        await enrollmentRepository.AddEnrollment(enrollment, cancellationToken);

        await integrationEventPublisher.PublishAsync(
            new EnrollmentCompletedEvent(
                enrollment.Id,
                enrollment.StudentId,
                enrollment.CourseId,
                enrollment.PaymentId,
                DateTime.UtcNow),
            cancellationToken);

        return Result.Success();
    }
}
