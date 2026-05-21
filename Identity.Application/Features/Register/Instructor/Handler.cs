using Identity.Application.Domain.DTO;
using Identity.Application.RepositoryInterfaces;
using MediatR;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Register.Instructor;

public class Handler : IRequestHandler<InstructorRegisterCommand, Result<LoginResponse>>
{
    private readonly IGenericRepository<Domain.Entities.Instructor> _repository;
    private readonly UserRegistrationService _registrationService;

    public Handler(IGenericRepository<Domain.Entities.Instructor> repository,
        UserRegistrationService registrationService)
    {
        _repository = repository;
        _registrationService = registrationService;
    }
    public async Task<Result<LoginResponse>> Handle(InstructorRegisterCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _registrationService.CreateUserAsync(
            request.Credentials.Email, request.Credentials.Password, Domain.AppRoles.Instructor, cancellationToken);

        if (!userResult.IsSuccess)
            return Result<LoginResponse>.Failure(userResult.Error!);

        var instructor = Domain.Entities.Instructor.Create(
            request.Credentials.FirstName, request.Credentials.LastName, userResult.Value!.Id, 
            request.Credentials.ProfilePictureUrl, request.Credentials.Bio);

        await _repository.AddAsync(instructor, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return await _registrationService.GenerateTokensAsync(userResult.Value!, cancellationToken);
    }
}
