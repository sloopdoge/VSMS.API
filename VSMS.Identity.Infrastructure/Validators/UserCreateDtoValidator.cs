using FluentValidation;
using VSMS.Identity.Domain.DTOs;

namespace VSMS.Identity.Infrastructure.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        // Email: not empty and valid format
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        // Username: no spaces/specials, only letters and numbers, not empty
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .Matches("^[a-zA-Z0-9]+$").WithMessage("Username can only contain letters and numbers without spaces.");
    }
}