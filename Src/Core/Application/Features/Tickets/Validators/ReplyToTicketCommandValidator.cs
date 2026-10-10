using Application.Features.Tickets.Commands;
using FluentValidation;

namespace Application.Features.Tickets.Validators;

public sealed class ReplyToTicketCommandValidator : AbstractValidator<ReplyToTicketCommand>
{
    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;
    private const int MaxAttachments = 5;

    public ReplyToTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
        .NotEmpty();

        RuleFor(x => x.Message)
        .NotEmpty()
        .MaximumLength(2000);

        RuleFor(x => x.Attachments)
            .Must(a => a.Count <= MaxAttachments)
            .WithMessage($"Maximum of {MaxAttachments} attachments allowed.");


        RuleForEach(x => x.Attachments).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.FileSize)
                .GreaterThan(0)
                .WithMessage("File is empty.")
                .LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage($"File size must not exceed {MaxFileSizeBytes / 1024 / 1024}MB.");

            attachment.RuleFor(a => a.ContentType)
                .Must(ct => AllowedContentTypes.Contains(ct.ToLowerInvariant()))
                .WithMessage("Only JPEG, PNG, or WEBP images are allowed.");
        });
    }

}
