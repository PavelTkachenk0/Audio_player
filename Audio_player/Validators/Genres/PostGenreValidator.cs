using Audio_player.Models.Requests;
using FluentValidation;

namespace Audio_player.Validators.Genres;

public class PostGenreValidator : BaseFileValidator<CreateGenreRequest>
{
    public PostGenreValidator() : base()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Cover)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(CheckExtensions)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght)
            .WithMessage("cover_is_too_large");
    }
}
