using FluentValidation;
using WebApplication.Models;
using WebApplication.Managers;

namespace WebApplication.Validators
{
    public class AuthValidator : AbstractValidator<Account>
    {
        public AuthValidator(IUserManager manager)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("E-mail is required");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");

            RuleFor(x => x)
                .CustomAsync(async (a, b, c) =>
                {
                    if (string.IsNullOrEmpty(a.Email) || string.IsNullOrEmpty(a.Password))
                        return;

                    var user = await manager.PasswordSignInAsync(a.Email, a.Password);

                    if (user == null) b.AddFailure("Login invalid");
                });
        }
    }
}
