using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Validators.Authentication;

public class RegistrationValidator : BaseValidator<RegisterRequest>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("email_cant_be_empty")
            .EmailAddress()
            .WithMessage("email_is_invalid")
            .MustAsync(async (val, ct) => !await DbContext.AppUsers.AnyAsync(x => x.Email == val, ct))
            .WithMessage("user_is_already_exists");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("password_cant_be_empty")
            .MinimumLength(8)
            .WithMessage("password_must_be_at_least_8_characters");
    }
}
