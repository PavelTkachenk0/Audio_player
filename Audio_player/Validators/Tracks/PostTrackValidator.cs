using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Validators.Tracks;

public class PostTrackValidator : BaseAudioFileValidator<PostTrackRequest>
{
    public PostTrackValidator() : base()
    {
        RuleFor(x => x.SongName)
            .NotEmpty();

        RuleFor(x => x.AlbumId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (val, ct) => await DbContext.Albums.AnyAsync(x => x.Id == val, ct))
            .WithMessage("album_not_found");

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

        RuleFor(x => x.SongFile)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(CheckExtensions)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght)
            .WithMessage("cover_is_too_large");
    }
}
