using FluentValidation;
using Identity.Application.DTOs.Permissions;

namespace Identity.Application.Validators
{
    public class PermissionFilterValidator : AbstractValidator<PermissionFilterDto>
    {
        private const int MaxPageSize = 200;

        public PermissionFilterValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, MaxPageSize)
                .WithMessage($"PageSize must be between 1 and {MaxPageSize}.");

            RuleFor(x => x.SortDir)
                .Must(d => string.IsNullOrEmpty(d) || d.Equals("asc", StringComparison.InvariantCultureIgnoreCase) || d.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("SortDir must be 'asc' or 'desc'.");

            RuleFor(x => x.SortBy)
                .MaximumLength(50);
        }
    }
}
