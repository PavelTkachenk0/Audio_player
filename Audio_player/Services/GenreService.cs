using Audio_player.DAL;
using Audio_player.Mappers;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Services;

public class GenreService(AppDbContext appDbContext, FileService fileService)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public async Task<List<GenreDTO>> GetByNameAsync(string? name, CancellationToken ct)
    {
        return await _appDbContext.Genres
            .Where(x => EF.Functions.ILike(x.Name, $"%{name}%"))
            .EntityToDto()
            .ToListAsync(ct);
    }

    public async Task<GenreDTO?> GetByIdAsync(short id, CancellationToken ct)
    {
        return await _appDbContext.Genres
            .Where(x => x.Id == id)
            .EntityToDto()
            .SingleOrDefaultAsync(ct);
    }

    public async Task CreateAsync(CreateGenreRequest req, CancellationToken ct)
    {
        var coverPath = await _fileService.CreateFile(req.Cover, true, ct);

        _appDbContext.Genres.Add(new DAL.Models.Genre
        {
            Name = req.Name,
            CoverPath = coverPath
        });

        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> UpdateAsync(short id, EditGenreRequest req, CancellationToken ct)
    {
        var genre = await _appDbContext.Genres.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (genre == null)
        {
            return false;
        }

        genre.Name = req.Name;

        if (req.Cover != null)
        {
            genre.CoverPath = await _fileService.CreateFile(req.Cover, true, ct);
        }

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(short id, CancellationToken ct)
    {
        var genre = await _appDbContext.Genres.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (genre == null)
        {
            return false;
        }

        _appDbContext.Genres.Remove(genre);
        _fileService.DeleteFile(genre.CoverPath);

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }
}
