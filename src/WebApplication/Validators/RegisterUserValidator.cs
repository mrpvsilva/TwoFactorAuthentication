using FluentValidation;
using WebApplication.Data;
using WebApplication.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Validators
{
    public class RegisterUserValidator : AbstractValidator<User>
    {
        public RegisterUserValidator(TFAContext ctx)
        {
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage("E-mail is required")
                .EmailAddress()
                .WithMessage("E-mail is invalid")
                .MustAsync(async (a, b, c) => !await ctx.Users.AnyAsync(x => x.Email == b))
                .WithMessage("E-mail already registered");          
        }
    }
}
