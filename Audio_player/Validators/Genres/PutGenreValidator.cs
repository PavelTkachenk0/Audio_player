using Audio_player.Models.Requests;
using FluentValidation;

namespace Audio_player.Validators.Genres;

public class PutGenreValidator : BaseImageFileValidator<EditGenreRequest>
{
    public PutGenreValidator() : base()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Cover)
            .Cascade(CascadeMode.Stop)
            .Must(CheckExtensions!).When(x => x.Cover != null)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght!).When(x => x.Cover != null)
            .WithMessage("cover_is_too_large");
    }
}
