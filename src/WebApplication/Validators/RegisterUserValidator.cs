using FluentValidation;
using WebApplication.Data;
using WebApplication.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Validators
{
    public class RegisterUserValidator : AbstractValidator<User>
    {
        public RegisterUserValidator(TfaContext ctx)
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("E-mail is required")
                .EmailAddress()
                .WithMessage("E-mail format is invalid")
                .MustAsync(async (email, ct) => !await ctx.Users.AnyAsync(x => x.Email == email))
                .WithMessage("This e-mail is already registered");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character");
        }
    }
}
