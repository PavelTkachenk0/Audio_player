using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Validators.Artists;

public class PostArtistValidator : BaseFileValidator<PostArtistRequest>
{
    public PostArtistValidator() : base()
    {
        RuleFor(x => x.ArtistName)
            .NotEmpty();

        RuleFor(x => x.GenreIds)
            .MustAsync(async (val, ct) => await DbContext.Genres.CountAsync(g => val.Contains(g.Id), ct) == val.Count)
            .WithMessage("one_or_more_genres_not_found");

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
