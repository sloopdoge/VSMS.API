using FluentValidation;
using VSMS.Identity.Domain.DTOs;

namespace VSMS.Identity.Infrastructure.Validators;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        // Email must not be empty and must follow email format
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        // Password must not be empty
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}