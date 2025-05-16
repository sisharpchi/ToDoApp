using FluentValidation;
using ToDoList.Bll.DTOs;

namespace ToDoList.Bll.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.")
            .Matches(@"^[A-Z][a-zA-Z]{1,49}$").WithMessage("First name must start with a capital letter and contain only letters.");

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.")
            .Matches(@"^[A-Z][a-zA-Z]{1,49}$").When(x => !string.IsNullOrWhiteSpace(x.LastName))
            .WithMessage("Last name must start with a capital letter and contain only letters.");

        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
            .Matches(@"^[a-zA-Z0-9_]{3,50}$").WithMessage("Username must be between 3 and 50 characters and contain only letters, numbers, or underscores.");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(320).WithMessage("Email cannot exceed 320 characters.")
            .EmailAddress().WithMessage("Email format is invalid.");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(128).WithMessage("Password cannot exceed 128 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$")
            .WithMessage("Password must be at least 8 characters long and include uppercase letters, lowercase letters, numbers, and special characters.");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters.")
            .Matches(@"^\+998\d{9}$").WithMessage("Phone number must start with +998 and be followed by 9 digits.");
    }
}
