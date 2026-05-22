using Courses.Application.Utilities;

namespace Courses.Application.Features.Create.Sections;

internal class CreateVideoItemValidator : AbstractValidator<CreateVideoItemCommand>
{
    public CreateVideoItemValidator()
    {
        RuleFor(x => x.Details.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Details.Duration)
            .NotEmpty();

        RuleFor(x => x.Details.SectionId)
            .NotEmpty();

        RuleFor(x => x.Details.VideoUrl)
            .NotEmpty()
            .Must(UrlValidator.BeValidUrl)
            .WithMessage("Video Url is not valid.");
    }
}

internal class CreatePdfItemValidator : AbstractValidator<CreatePdfItemCommand>
{
    public CreatePdfItemValidator()
    {
        RuleFor(x => x.Details.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Details.SectionId)
            .NotEmpty();

        RuleFor(x => x.Details.PdfUrl)
            .NotEmpty()
            .Must(UrlValidator.BeValidUrl)
            .WithMessage("Pdf Url is not valid.");
    }
}
