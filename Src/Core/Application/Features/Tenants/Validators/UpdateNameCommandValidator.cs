using Application.Features.Tenants.Commands;
using FluentValidation;

namespace Application.Features.Tenants.Validators
{
    public sealed class UpdateNameCommandValidator : AbstractValidator<UpdateNameCommand>
    {
        public UpdateNameCommandValidator()
        {
            RuleFor(x => x.TenantName)
                .MaximumLength(150)
                .NotEmpty();
        }
    }
}
