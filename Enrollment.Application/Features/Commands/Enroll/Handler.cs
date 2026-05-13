using Enrollment.Application.Repositories;
using MediatR;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Result;

namespace Enrollment.Application.Features.Commands.Enroll;

internal sealed class Handler(
    ICourseService courseService,
    IStudentService studentService,
    IEnrollmentRepository enrollmentRepository
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

        // TODO: is already enrolled (prevent duplication)


        // TODO:check if the user payment succeeded


        Domain.Entities.Enrollment enrollment;
        try
        {
            enrollment = Domain.Entities.Enrollment.Create(dto.StudentId, dto.CourseId, dto.Status);
        }
        catch (InvalidOperationException ex)
        {

            return Result.Failure(ex.Message);
        }


        await enrollmentRepository.AddEnrollment(enrollment, cancellationToken);

        return Result.Success();
    }
}
