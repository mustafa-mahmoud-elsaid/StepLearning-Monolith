using Identity.Application.Domain.DTO;
using Identity.Application.Infrastructure;
using Identity.Application.Infrastructure.JWT;
using Identity.Application.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Register.Instructor;

public class Handler : IRequestHandler<InstructorRegisterCommand, Result<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenericRepository<Domain.Entities.Instructor> _repository;
    private readonly ITokenService _tokenService;

    public Handler(UserManager<ApplicationUser> userManager, IGenericRepository<Domain.Entities.Instructor> repository,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _repository = repository;
        _tokenService = tokenService;
    }
    public async Task<Result<LoginResponse>> Handle(InstructorRegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.dto.Email);

        if (user is not null)
            return Result<LoginResponse>.Failure("Email already exists");

        var dto = request.dto;

        try
        {
            var appUser = ApplicationUser.Create(dto.Email);
            var result = await _userManager.CreateAsync(appUser, dto.Password);

            if (!result.Succeeded)
                return Result<LoginResponse>.Failure("Failed to register the user");

            var instructor = Domain.Entities.Instructor
                .Create(dto.FirstName, dto.LastName, appUser.Id, dto.ProfilePictureUrl, dto.Bio);

            // add instructor
            await _repository.AddAsync(instructor, cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);

            var jwtToken = await _tokenService.GenerateJWTToken(appUser);

            var refTokenResult = await _tokenService.GenerateRefreshToken(appUser, cancellationToken);

            if (!refTokenResult.IsSuccess)
                return Result<LoginResponse>.Failure(refTokenResult.Error!);

            return Result<LoginResponse>.Success(new(jwtToken, refTokenResult.Value!));

        }
        catch (InvalidOperationException ex)
        {
            return Result<LoginResponse>.Failure(ex.Message);
        }
       
    }
}
