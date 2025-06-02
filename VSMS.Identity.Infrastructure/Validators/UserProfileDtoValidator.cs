using FluentValidation;
using VSMS.Identity.Domain.DTOs;

namespace VSMS.Identity.Infrastructure.Validators;

public class UserProfileDtoValidator  : AbstractValidator<UserProfileDto>
{
    public UserProfileDtoValidator()
    {
        // 1. Username: no spaces/specials, only letters and numbers, not empty
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .Matches("^[a-zA-Z0-9]+$").WithMessage("Username can only contain letters and numbers without spaces.");

        // 2. FirstName: only letters, no spaces
        RuleFor(x => x.FirstName)
            .Matches("^[a-zA-Z]*$").WithMessage("First name can only contain letters.");

        // 3. LastName: same as FirstName
        RuleFor(x => x.LastName)
            .Matches("^[a-zA-Z]*$").WithMessage("Last name can only contain letters.");

        // 4. Email: not empty and valid format
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        // 5. PhoneNumber: only digits
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d*$").WithMessage("Phone number must contain only digits.");
    }
}