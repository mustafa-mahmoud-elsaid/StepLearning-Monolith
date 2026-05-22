using Identity.Application.DTO;
using Identity.Application.RepositoryInterfaces;

namespace Identity.Application.Features.Register.Instructor;
internal sealed class Handler(IGenericRepository<Domain.Entities.Instructor> repository,
        UserRegistrationService registrationService) : IRequestHandler<InstructorRegisterCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(InstructorRegisterCommand request, CancellationToken cancellationToken)
    {
        var userResult = await registrationService.CreateUserAsync(
            request.Credentials.Email, request.Credentials.Password, Domain.AppRoles.Instructor, cancellationToken);

        if (!userResult.IsSuccess)
            return Result<LoginResponse>.Failure(userResult.Error!);

        var instructor = Domain.Entities.Instructor.Create(
            request.Credentials.FirstName, request.Credentials.LastName, userResult.Value, 
            request.Credentials.ProfilePictureUrl, request.Credentials.Bio);

        await repository.AddAsync(instructor, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return await registrationService.GenerateTokensAsync(userResult.Value!, cancellationToken);
    }
}


