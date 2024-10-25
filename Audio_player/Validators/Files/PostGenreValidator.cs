using Audio_player.Models.Requests;
using FluentValidation;

namespace Audio_player.Validators.Files;

public class PostGenreValidator : BasePostImageValidator<CreateGenreRequest>
{
    public PostGenreValidator() : base()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
