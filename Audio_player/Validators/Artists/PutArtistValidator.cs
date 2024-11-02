using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Validators.Artists;

public class PutArtistValidator : BaseFileValidator<PutArtistRequest>
{
    public PutArtistValidator() : base()
    {
        RuleFor(x => x.ArtistName)
            .NotEmpty();

        RuleFor(x => x.GenreIds)
            .MustAsync(async (val, ct) => await DbContext.Genres.CountAsync(g => val.Contains(g.Id), ct) == val.Count)
            .WithMessage("one_or_more_genres_not_found");

        RuleFor(x => x.Cover)
            .Cascade(CascadeMode.Stop)
            .Must(CheckExtensions!).When(x => x.Cover != null)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght!).When(x => x.Cover != null)
            .WithMessage("cover_is_too_large");

        RuleFor(x => x.Avatar)
            .Cascade(CascadeMode.Stop)
            .Must(CheckExtensions!).When(x => x.Avatar != null)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght!).When(x => x.Avatar != null)
            .WithMessage("avatar_is_too_large");
    }
}
