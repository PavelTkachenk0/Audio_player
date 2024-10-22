using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Validators.Authentification;

public class RegistrationValidator : BaseValidator<RegisterRequest>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("email_cant't_be_empty")
            .MustAsync(async (val, ct) => !await DbContext.AppUsers.AnyAsync(x => x.Email == val, ct))
            .WithMessage("user_is_already_exists");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("email_cant't_be_empty");
    }
}
