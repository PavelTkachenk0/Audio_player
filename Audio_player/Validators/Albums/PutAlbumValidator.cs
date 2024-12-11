using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Validators.Albums;

public class PutAlbumValidator : BaseImageFileValidator<PutAlbumRequest>
{
    public PutAlbumValidator() : base()
    {
        RuleFor(x => x.AlbumName)
            .NotEmpty();

        RuleFor(x => x.Cover)
            .Cascade(CascadeMode.Stop)
            .Must(CheckExtensions!).When(x => x.Cover != null)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght!).When(x => x.Cover != null)
            .WithMessage("cover_is_too_large");

        RuleFor(x => x.GenreIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (val, ct) => await DbContext.Genres.CountAsync(g => val.Contains(g.Id), ct) == val.Count)
            .WithMessage("one_or_more_genres_not_found");

        RuleFor(x => x.ArtistIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (val, ct) => await DbContext.Artists.CountAsync(a => val.Contains(a.Id), ct) == val.Count)
            .WithMessage("one_or_more_artists_not_found");
    }
}
