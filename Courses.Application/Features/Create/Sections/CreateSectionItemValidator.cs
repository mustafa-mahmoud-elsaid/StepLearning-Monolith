using Courses.Application.Utilities;
using FluentValidation;

namespace Courses.Application.Features.Create.Sections;

internal class CreateVideoItemValidator : AbstractValidator<CreateVideoItemCommand>
{
    public CreateVideoItemValidator()
    {
        RuleFor(x => x.dto.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.dto.Duration)
            .NotEmpty();

        RuleFor(x => x.dto.SectionId)
            .NotEmpty();

        RuleFor(x => x.dto.VideoUrl)
            .NotEmpty()
            .Must(Helper.BeValidUrl)
            .WithMessage("Video Url is not valid.");
    }
}

internal class CreatePdfItemValidator : AbstractValidator<CreatePdfItemCommand>
{
    public CreatePdfItemValidator()
    {
        RuleFor(x => x.dto.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.dto.SectionId)
            .NotEmpty();

        RuleFor(x => x.dto.PdfUrl)
            .NotEmpty()
            .Must(Helper.BeValidUrl)
            .WithMessage("Pdf Url is not valid.");
    }
}
