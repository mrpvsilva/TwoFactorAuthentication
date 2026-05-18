using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Validators
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPassword>
    {
        public ForgotPasswordValidator(TfaContext ctx)
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("E-mail is required")
                .EmailAddress()
                .WithMessage("E-mail format is invalid")
                .MustAsync(async (email, ct) => await ctx.Users.AnyAsync(x => x.Email == email))
                .WithMessage("No account found with this e-mail");
        }
    }
}
