using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.SharedAuth;
using FluentValidation;

namespace BookShop.Auth.DataAuth.Validators;


public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.username)
            .NotEmpty()
            .WithMessage("Username is required")
            .NotNull()
            .WithMessage("Username is required")
            .Matches(RegexPattern.UserName)
            .WithMessage("Username must be at least 6 characters long and contain only letters, numbers, underscores, and hyphens");

        RuleFor(x => x.password)
            .NotEmpty()
            .WithMessage("Password is required")
            .NotNull()
            .WithMessage("Password is required")
            .Matches(RegexPattern.Password)
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one number");
    }
}