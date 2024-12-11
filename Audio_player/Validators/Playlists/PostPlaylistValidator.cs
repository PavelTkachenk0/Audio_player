using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Validators.Playlists;

public class PostPlaylistValidator : BaseImageFileValidator<PostPlaylistRequest>
{
    public PostPlaylistValidator() : base()
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

        RuleFor(x => x.TrackIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (val, ct) => await DbContext.Songs.CountAsync(g => val.Contains(g.Id), ct) == val.Count)
            .WithMessage("one_or_more_track_not_found");
    }
}
