using FluentValidation;
using WebApplication.Models;

namespace WebApplication.Validators
{
    public class VerifyResetCodeValidator : AbstractValidator<VerifyResetCode>
    {
        public VerifyResetCodeValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("E-mail is required")
                .EmailAddress()
                .WithMessage("E-mail format is invalid");

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Code is required")
                .Length(6)
                .WithMessage("Code must be exactly 6 digits")
                .Matches("^[0-9]{6}$")
                .WithMessage("Code must contain only numbers");
        }
    }
}
