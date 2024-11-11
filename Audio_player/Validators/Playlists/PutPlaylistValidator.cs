using Audio_player.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Validators.Playlists;

public class PutPlaylistValidator : BaseImageFileValidator<PutPlaylistRequest>
{
    public PutPlaylistValidator() : base()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Cover)
            .Cascade(CascadeMode.Stop)
            .Must(CheckExtensions!).When(x => x.Cover != null)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght!).When(x => x.Cover != null)
            .WithMessage("cover_is_too_large");

        RuleFor(x => x.TrackIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (val, ct) => await DbContext.Songs.CountAsync(g => val.Contains(g.Id), ct) == val.Count)
            .WithMessage("one_or_more_track_not_found");
    }
}
