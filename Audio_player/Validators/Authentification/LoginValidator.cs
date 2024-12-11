using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Validators.Authentification;

public class LoginValidator : BaseValidator<LoginRequest>
{
    public LoginValidator()
    {

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("email_cant't_be_empty")
            .MustAsync(async (val, ct) => await DbContext.AppUsers.AnyAsync(x => x.Email == val, ct))
            .WithMessage("user_is't_registered");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("email_cant't_be_empty");
    }
}
