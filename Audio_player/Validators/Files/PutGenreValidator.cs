using Audio_player.Models.Requests;
using FluentValidation;

namespace Audio_player.Validators.Files;

public class PutGenreValidator : BasePostImageValidator<EditGenreRequest>
{
    public PutGenreValidator() : base()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
