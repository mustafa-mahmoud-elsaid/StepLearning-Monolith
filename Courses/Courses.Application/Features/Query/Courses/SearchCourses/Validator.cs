
namespace Courses.Application.Features.Query.Courses.SearchCourses;

internal sealed class SearchCoursesQueryValidator : AbstractValidator<SearchCoursesQuery>
{
    public SearchCoursesQueryValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(250)
            .When(x => x.Title is not null)
            .WithMessage("Search title must not exceed 250 characters.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum price must not be negative.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue)
            .WithMessage("Maximum price must not be negative.");

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
            .WithMessage("Minimum price must not exceed maximum price.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");
    }
}
