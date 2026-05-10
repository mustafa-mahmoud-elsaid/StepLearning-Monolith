using Identity.Application.Domain.DTO;
using Identity.Application.Infrastructure;
using Identity.Application.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Register.Student;

public sealed class Handler(
    IGenericRepository<Domain.Entities.Student> repository,
    UserManager<ApplicationUser> userManager) 
    : IRequestHandler<StudentRegisterCommand, Result<LoginResponse>>
{
    private readonly IGenericRepository<Domain.Entities.Student> _repository = repository;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<LoginResponse>> Handle(StudentRegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.dto;

        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is not null)
            return Result<LoginResponse>.Failure("Email already exists");

        try
        {
            var appUser = ApplicationUser.Create(dto.Email);
            var result = await _userManager.CreateAsync(appUser, dto.Password);

            if (!result.Succeeded)
                return Result<LoginResponse>.Failure("Failed to register the user");

            var student = Domain.Entities.Student
                .Create(dto.FullName, appUser.Id, dto.DateOfBirth, dto.ProfilePictureUrl);

            // add instructor
            await _repository.AddAsync(student, cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);

            return Result<LoginResponse>.Success(new("token", "refreshToken"));

        }
        catch (InvalidOperationException ex)
        {
            return Result<LoginResponse>.Failure(ex.Message);
        }
    }
}
