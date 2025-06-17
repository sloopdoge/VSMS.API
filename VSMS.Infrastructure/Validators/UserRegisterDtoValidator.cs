using FluentValidation;
using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Validators;

public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator()
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

        // 6. Password: 
        //   - at least 8 characters
        //   - 1 uppercase
        //   - 1 number
        //   - 1 special character
        //   - no spaces
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.")
            .Matches("^[^\\s]+$").WithMessage("Password must not contain spaces.");
    }
}