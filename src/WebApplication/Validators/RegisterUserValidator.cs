using FluentValidation;
using WebApplication.Entities;

namespace WebApplication.Validators
{
    public class RegisterUserValidator : AbstractValidator<User>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email is invalid");

            RuleFor(x => x.Key)
                .NotEmpty()
                .WithMessage("Key is required");
        }
    }
}
