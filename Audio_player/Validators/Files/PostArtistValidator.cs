using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Validators.Files;

public class PostArtistValidator : BaseFileValidator<PostArtistRequest>
{
    public PostArtistValidator() : base()
    {
        RuleFor(x => x.ArtistName)
            .NotEmpty();

        RuleFor(x => x.GenreId)
            .MustAsync(async (val, ct) => await DbContext.Genres.AnyAsync(x => x.Id == val, ct))
            .WithMessage("genre_is_not_found");

        RuleFor(x => x.Cover)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(CheckExtensions)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght)
            .WithMessage("cover_is_too_large");

        RuleFor(x => x.Avatar)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(CheckExtensions)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght)
            .WithMessage("avatar_is_too_large");
    }
}
