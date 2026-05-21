using Identity.Application.Domain.DTO;
using Identity.Application.RepositoryInterfaces;
using MediatR;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Register.Student;

internal sealed class Handler(
    IGenericRepository<Domain.Entities.Student> repository,
    UserRegistrationService registrationService) 
    : IRequestHandler<StudentRegisterCommand, Result<LoginResponse>>
{
    private readonly IGenericRepository<Domain.Entities.Student> _repository = repository;
    private readonly UserRegistrationService _registrationService = registrationService;

    public async Task<Result<LoginResponse>> Handle(StudentRegisterCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _registrationService.CreateUserAsync(
            request.Credentials.Email, request.Credentials.Password, Domain.AppRoles.Student, cancellationToken);

        if (!userResult.IsSuccess)
            return Result<LoginResponse>.Failure(userResult.Error!);

        var student = Domain.Entities.Student.Create(
            request.Credentials.FullName, userResult.Value!.Id,
            request.Credentials.DateOfBirth, request.Credentials.ProfilePictureUrl);

        await _repository.AddAsync(student, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return await _registrationService.GenerateTokensAsync(userResult.Value!, cancellationToken);
    }
}
